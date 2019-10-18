using ShipEngine.ApiClient.Api;
using ShipEngine.ApiClient.Model;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.Sample.ShipEngine.Policies
{
    public class ShipEnginePolicy : Policy
    {
        private Carrier _defaultCarrier;
        private List<Carrier> _carriers;

        public IDictionary<string, string> UnitMapping { get; private set; }

        public IList<String> ShippingMethods { get; private set; }

        public ShipEnginePolicy()
        {
            ShippingMethods = new List<String>();
        }

        public string ApiKey { get; set; }

        public List<Carrier> Carriers
        {
            get
            {
                if (this._carriers == null)
                {
                    var carrierApi = new CarriersApi();
                    this._carriers = carrierApi.CarriersList(this.ApiKey).Carriers;
                }
                return this._carriers;
            }

        }

        private string _defaultCarrierNameContains;

        public string DefaultCarrierNameContains
        {
            get
            {
                return _defaultCarrierNameContains;
            }

            set
            {
                _defaultCarrierNameContains = value;
                this._defaultCarrier = this.Carriers.FirstOrDefault<Carrier>(
                c => c.Nickname.IndexOf(_defaultCarrierNameContains, StringComparison.CurrentCultureIgnoreCase) >= 0
                );

                if (this._defaultCarrier == null)
                {
                    this._defaultCarrier = this.Carriers.FirstOrDefault<Carrier>(
                   c => c.FriendlyName.IndexOf(_defaultCarrierNameContains, StringComparison.CurrentCultureIgnoreCase) >= 0
                   ) ?? this.Carriers.First();
                }
            }
        }
        public virtual Carrier DefaultCarrier
        {
            get
            {
                return _defaultCarrier;
            }

            private set
            {
                _defaultCarrier = value;
                this._defaultCarrierNameContains = _defaultCarrier.FriendlyName;
            }
        }



        public virtual Dimensions.UnitEnum StringToDimensionUnit(string unitOfMeasure)
        {
            char c = string.IsNullOrWhiteSpace(unitOfMeasure) ? '\0' : unitOfMeasure.FirstOrDefault<char>(ch => char.IsLetter(ch));

            switch (c)
            {
                case 'i': /* in */
                case 'I': return Dimensions.UnitEnum.Inch;

                case 'c': /* cm */
                case 'C': return Dimensions.UnitEnum.Centimeter;

                default: throw new Exception(string.Format("Unknown lenght unit '%s'.  Expecting 'Inch' or 'Centimeter'", unitOfMeasure));
            }


        }
        public virtual Weight.UnitEnum StringToWeightUnit(string unitOfWeight)
        {
            char c = string.IsNullOrWhiteSpace(unitOfWeight) ? '\0' : unitOfWeight.FirstOrDefault<char>(ch => char.IsLetter(ch));

            switch (c)
            {
                case 'o': /* oz */
                case 'O': return Weight.UnitEnum.Ounce;

                case 'l': /* lb */
                case 'L':
                case 'p': /* pnd */
                case 'P': return Weight.UnitEnum.Pound;

                case 'g':
                case 'G': return Weight.UnitEnum.Gram;

                case 'k': /* kilos */
                case 'K': return Weight.UnitEnum.Kilogram;

                default: throw new Exception(string.Format("Unknown lenght unit '%s'.  Expecting 'Ounce', 'Pound', 'Gram', 'Kilogram'", unitOfWeight));
            }

        }
        public virtual string GetWearhouseId(AddressDTO destination)
        {
            return DefaultWearhouseId;
        }
        public virtual AddressValidatingShipment.ConfirmationEnum GetShippingConfirmationMethod()
        {
            return ShipDeliveryConfirmation;
        }

        public virtual MoneyDTO GetInsuredAmount(SellableItem sellableItem, CommercePipelineExecutionContext context)
        {
            MoneyDTO.CurrencyEnum currencyCode;

            if (sellableItem == null 
            || sellableItem.ListPrice == null
            || MoneyDTO.CurrencyEnum.TryParse(sellableItem.ListPrice.CurrencyCode, out currencyCode) == false)
            {
                return null;
            }

            return new MoneyDTO( currencyCode, (double)sellableItem.ListPrice.Amount * ShipInsuranceAmountPercent / 100.0);

        }


        public AddressValidatingShipment.InsuranceProviderEnum ShipInsuranceProvider { get; set; }
        public AddressValidatingShipment.ConfirmationEnum ShipDeliveryConfirmation { get; set; }
        public int ShipInsuranceAmountPercent { get; set; }
        
        public string ShipperName {get; set;}
        public string ShipperCompany { get; set; }
        public string ShipperPhone { get; set; }
        public string ShipperAddressLine1 { get; set; }
        public string ShipperAddressLine2 { get; set; }
        public string ShipperAddressLine3 { get; set; }
        public string ShipperCityLocality { get; set; }
        public string ShipperCountryCode { get; set; }
        public string ShipperStateProvince { get; set; }
        public string ShipperPostalCode { get; set; }
        public string DefaultWearhouseId { get; set; }

        public AddressValidatingShipment.ValidateAddressEnum AddressValidationMethod { get; set; }

    }

}
