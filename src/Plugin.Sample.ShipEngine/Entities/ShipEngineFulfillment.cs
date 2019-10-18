using Microsoft.Extensions.Logging;
using ShipEngine.ApiClient.Api;
using ShipEngine.ApiClient.Model;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Fulfillment;
using Plugin.Sample.ShipEngine.Policies;
using StatesAndProvinces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace Plugin.Sample.ShipEngine.Entities
{
    public class ShipEngineFulfillment
    {

        public ShippingRateResult GetShippingRate(
             string shippingMethod,
             Cart cart,
             CommercePipelineExecutionContext context,
             GetSellableItemCommand getSellableItemCommand
            )
        {
            if (cart == null
            || !cart.HasComponent<PhysicalFulfillmentComponent>()
            || string.IsNullOrEmpty(shippingMethod)
            )
            {
                return null;
            }


            context.Logger.LogInformation($"{this.GetType().Name} - Begin GetShippingRate", Array.Empty<object>());



            var policy = context.CommerceContext.GetPolicy<ShipEnginePolicy>();
            var component = cart.GetComponent<PhysicalFulfillmentComponent>();
            var shippingParty = component?.ShippingParty;

            var countryCode = Get2DigitCountryCode(shippingParty.CountryCode);
            Carrier carrier = policy.DefaultCarrier;

            var shipment = new AddressValidatingShipment()
            {
                Confirmation = policy.ShipDeliveryConfirmation,

                InsuranceProvider = policy.ShipInsuranceProvider,

                CarrierId = carrier.CarrierId,

                ServiceCode = carrier.Services.FirstOrDefault().ServiceCode, // "usps_priority_mail" edvin change

                Packages = new List<ShipmentPackage>(),

                ShipFrom = new AddressDTO(
                    name: String.IsNullOrWhiteSpace(policy.ShipperName) ? context.CommerceContext.CurrentShopName() : policy.ShipperName,
                    phone: policy.ShipperPhone,
                    companyName: String.IsNullOrWhiteSpace(policy.ShipperCompany) ? context.CommerceContext.CurrentShopName() : policy.ShipperCompany,
                    addressLine1: policy.ShipperAddressLine1,
                    addressLine2: policy.ShipperAddressLine2,
                    addressLine3: policy.ShipperAddressLine3,
                    cityLocality: policy.ShipperCityLocality,
                    stateProvince: policy.ShipperStateProvince,
                    postalCode: policy.ShipperPostalCode,
                    countryCode: policy.ShipperCountryCode
                    ),

                ShipTo = new AddressDTO(
                    name: shippingParty.AddressName,
                    phone: shippingParty.PhoneNumber,
                    companyName: shippingParty.Name,
                    addressLine1: shippingParty.Address1,
                    addressLine2: shippingParty.Address2,
                    addressLine3: string.Empty,
                    cityLocality: shippingParty.City,
                    stateProvince: Get2DigitStateCode(countryCode, shippingParty.State),
                    postalCode: shippingParty.ZipPostalCode,
                    countryCode: countryCode
                   ),

            };


            // Address validation handled in ResolveAddressBlock
            shipment.ValidateAddress = AddressValidatingShipment.ValidateAddressEnum.NoValidation;

            shipment.WarehouseId = policy.GetWearhouseId(shipment.ShipTo);

            // Get sellableItem weight and dimensions
            //
            var shipmentPackages = new List<ShipmentPackage>();

            var dim = new Dimensions(Dimensions.UnitEnum.Inch);

            MoneyDTO insuredValue = null;

            foreach (var cartLineItem in cart.Lines)
            {

                var sellableItem = getSellableItemCommand.Process(context.CommerceContext, cartLineItem.ItemId, filterVariations: true).Result;

                // get specific weight value
                //
                var itemSpec = sellableItem.GetComponent<ItemSpecificationsComponent>();

                if (itemSpec == null || String.IsNullOrWhiteSpace(itemSpec.DimensionsUnitOfMeasure))
                {
                    var errorMessage = string.Format("{0} - no Item Specification (Weight, Dimension) exists for SellableItem {1}, productId {2}",
                        this.GetType().Name,
                        sellableItem.Name,
                        sellableItem.ProductId);


                    context.CommerceContext.AddMessage(
                       context.GetPolicy<KnownResultCodes>().Error,
                       "InvalidOrMissingPropertyValue",
                       new object[] { errorMessage })
                   ;

                    context.Logger.LogError(errorMessage);

                    return new ShippingRateResult()
                    {
                        ErrorMessage = errorMessage,
                        IsErrorAddedToCommerceContext = true
                    };

                }

                var globalPolicy = context.GetPolicy<GlobalPhysicalFulfillmentPolicy>();


                // Get the unit for dimension and weight as required by ShipEngine
                //



                // Handle edge case of Feet and Meters by converting them to inches and centimeters
                switch (itemSpec.DimensionsUnitOfMeasure.ToLower().Substring(0, 2))
                {
                    case "fe": // feet
                    case "fo": // foot

                        itemSpec.Length *= 12;
                        itemSpec.Width *= 12;
                        itemSpec.Height *= 12;
                        itemSpec.DimensionsUnitOfMeasure = "Inch";
                        break;

                    case "me": // Meters

                        itemSpec.Length *= 100;
                        itemSpec.Width *= 100;
                        itemSpec.Height *= 100;
                        itemSpec.DimensionsUnitOfMeasure = "Centimeter";
                        break;

                    case "mi": // Millimeter

                        itemSpec.Length = Math.Max(itemSpec.Length / 10, 1);
                        itemSpec.Width = Math.Max(itemSpec.Width / 10, 1);
                        itemSpec.Height = Math.Max(itemSpec.Height / 10, 1);
                        itemSpec.DimensionsUnitOfMeasure = "Centimeter";
                        break;
                }


                var dimensionUnit = policy.StringToDimensionUnit(
                        string.IsNullOrEmpty(itemSpec.DimensionsUnitOfMeasure)
                        ? globalPolicy.MeasurementUnits
                        : itemSpec.DimensionsUnitOfMeasure
                        );

                var weightUnit = policy.StringToWeightUnit(
                        string.IsNullOrEmpty(itemSpec.WeightUnitOfMeasure)
                        ? globalPolicy.WeightUnits
                        : itemSpec.WeightUnitOfMeasure
                        );




                if (policy.ShipInsuranceProvider != AddressValidatingShipment.InsuranceProviderEnum.None)
                {
                    insuredValue = policy.GetInsuredAmount(sellableItem, context);
                }


                if (!isSellableItemWeightWithInPolicyLimit(itemSpec, policy, globalPolicy))
                {
                    context.Logger.LogError(
                        string.Format("{0} - Item weight ({1}) Exceeding golbal policy MaximumShippingWeigh ({2}) for SellableItem {3}, productId {4}",
                        this.GetType().Name,
                        itemSpec.Weight,
                        globalPolicy.MaxShippingWeight,
                        sellableItem.Name,
                        sellableItem.ProductId)
                       );
                }

                shipmentPackages.Add(new ShipmentPackage
                {
                    Weight = new Weight(itemSpec.Weight, weightUnit),
                    Dimensions = new Dimensions(dimensionUnit, itemSpec.Length, itemSpec.Width, itemSpec.Height),
                    InsuredValue = insuredValue,
                    LabelMessages = null

                });


            }

            //Func<Rate, bool> shippingMethodMatchDelegate = rate => rate.ServiceCode?.IndexOf(shippingMethod, StringComparison.InvariantCultureIgnoreCase) >= 0;

            MoneyDTO shipmentAmount = null;

            // Get shipping rate if the fullfilment center supports multi-package shipment
            //
            RatesApi shippingRate = new RatesApi();
            if (carrier.HasMultiPackageSupportingServices ?? false)
            {
                shipment.Packages = shipmentPackages;

                var rateShipmentRequest = new RateShipmentRequest(shipment: shipment, rateOptions: new RateRequest(new List<string> { carrier.CarrierId }));

                rateShipmentRequest.Shipment.Confirmation = policy.GetShippingConfirmationMethod();

                var shipmentResponse = shippingRate.RatesRateShipment(rateShipmentRequest, policy.ApiKey);

                shipmentAmount = GetShippingResponseRatesAsync(shipmentResponse, shippingMethod, context).Result;


            }

            // If multi-package shipment is not supported; then we will get rate for
            // each package and and calculate the total sum as shipping rate.
            //
            else
            {

                var currency = (MoneyDTO.CurrencyEnum)Enum.Parse(typeof(MoneyDTO.CurrencyEnum), context.CommerceContext.CurrentCurrency());

                shipmentAmount = new MoneyDTO(currency, amount: 0);
                foreach (var package in shipmentPackages)
                {

                    shipment.Packages.RemoveAll(s => true);
                    shipment.Packages.Add(package);

                    var rateShipmentRequest = new RateShipmentRequest(shipment: shipment, rateOptions: new RateRequest(new List<string> { carrier.CarrierId }));

                    var shipmentRate = shippingRate.RatesRateShipment(rateShipmentRequest, policy.ApiKey);

                    var amount = GetShippingResponseRatesAsync(shipmentRate, shippingMethod, context).Result;

                    if (amount == null)
                    {
                        return null;
                    }
                    shipmentAmount.Amount += amount.Amount;
                    insuredValue.Amount += package.InsuredValue.Amount;

                }
            }
            context.Logger.LogInformation($"{this.GetType().Name} - End GetShippingRate", Array.Empty<object>());


            return new ShippingRateResult()
            {
                ShippingRate = shipmentAmount,
                InsuranceAmount = insuredValue
            };

        }

        private async Task<MoneyDTO> GetShippingResponseRatesAsync(RateShipmentResponse shipmentResponse, string shippingMethod, CommercePipelineExecutionContext context)
        {
            // Func<Rate, bool> shippingMethodMatchDelegate = rate => rate.ServiceCode?.IndexOf(shippingMethod, StringComparison.InvariantCultureIgnoreCase) >= 0;

            var shipmentDetail =
                shipmentResponse
                .RateResponse
                .Rates
                .FirstOrDefault(r => r.ServiceCode?.IndexOf(shippingMethod, StringComparison.InvariantCultureIgnoreCase) >= 0);

            if (shipmentDetail == null)
            {
                if (shipmentResponse.RateResponse.Errors.Count > 0)
                {
                    for (int i = 0; i < shipmentResponse.RateResponse.Errors.Count; i++)
                    {
                        context.Abort(await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Error,
                        "UnavailableServiceType",
                        new object[1] { shipmentResponse.RateResponse.Errors[i].Message }
                        ).ConfigureAwait(continueOnCapturedContext: false), context);
                    }
                }

                return null;
            }


            return shipmentDetail.ShippingAmount;
        }

        private bool isSellableItemWeightWithInPolicyLimit(
            ItemSpecificationsComponent itemSpec,
            ShipEnginePolicy localPolicy,
            GlobalPhysicalFulfillmentPolicy globalPolicy)
        {
            if (itemSpec == null
            || string.IsNullOrWhiteSpace(itemSpec.WeightUnitOfMeasure)
            || globalPolicy == null
            || string.IsNullOrWhiteSpace(globalPolicy.WeightUnits))
            {
                return true;
            }

            var globalWeightUnits = localPolicy.StringToWeightUnit(globalPolicy.WeightUnits);

            var itemWeightUnits = localPolicy.StringToWeightUnit(itemSpec.WeightUnitOfMeasure);

            return ToGrams(itemWeightUnits, itemSpec.Weight) <= ToGrams(globalWeightUnits, (double)globalPolicy.MaxShippingWeight);
        }


        private static double ToGrams(Weight.UnitEnum weightUnit, double mass)
        {
            switch (weightUnit)
            {
                case Weight.UnitEnum.Gram: return mass; ;
                case Weight.UnitEnum.Kilogram: return mass / 1000.0;
                case Weight.UnitEnum.Ounce: return mass * 28.3495;
                case Weight.UnitEnum.Pound: return mass * 453.592;
                default: return mass;
            }

        }


        private static string Get2DigitStateCode(string twoDigitCountryCode, string state)
        {
            List<SubRegion> regions = null;

            switch (twoDigitCountryCode.ToUpperInvariant())
            {
                case "US":
                    regions = Factory.Make(CountrySelection.UnitedStates);
                    break;

                case "CA":
                    regions = Factory.Make(CountrySelection.Canada);
                    break;

                default: return state;
            }

            var region = regions.FirstOrDefault<SubRegion>(r => r.Name.Equals(state, StringComparison.CurrentCultureIgnoreCase));

            return region == null ? state : region.Abbreviation;
        }

        private static string Get2DigitCountryCode(string country)
        {
            if (string.IsNullOrEmpty(country))
            {
                return string.Empty;
            }

            var destRegion = new RegionInfo(country);
            return (destRegion == null) ? country : destRegion.TwoLetterISORegionName;
        }
    }
}
