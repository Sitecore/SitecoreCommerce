using Microsoft.Extensions.Logging;
using Plugin.Sample.ShipEngine.Entities;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Catalog;
using Sitecore.Commerce.Plugin.Fulfillment;
using Sitecore.Commerce.Plugin.Pricing;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.ShipEngine.Pipelines.Blocks
{
    public class ShipEngineCalculateCartFulfillmentBlock : PipelineBlock<Cart, Cart, CommercePipelineExecutionContext>
    {

        private readonly GetSellableItemCommand _getSellableItemCommand;

        public ShipEngineCalculateCartFulfillmentBlock(GetSellableItemCommand getSellableItemCommand)
        {
            _getSellableItemCommand = getSellableItemCommand;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<Cart> Run(Cart cart, CommercePipelineExecutionContext context)
        {

            Condition.Requires<Cart>(cart).IsNotNull<Cart>(base.Name + ": The cart cannot be null.");

            if (cart == null ||
                cart.Adjustments == null ||
                cart.Adjustments.Count <= 0 ||
                !cart.HasComponent<PhysicalFulfillmentComponent>())
            {
                return await Task.FromResult(cart);
            }

            // Check to see if there is existing fulfillment fee block to replace
            var defaultFulfillmentAdjustment = cart.Adjustments.FirstOrDefault(a =>
                   (a.Name.Equals("FulfillmentFee", StringComparison.OrdinalIgnoreCase)
                   && (!string.IsNullOrEmpty(a.AdjustmentType)
                   && a.AdjustmentType.Equals(context.GetPolicy<KnownCartAdjustmentTypesPolicy>().Fulfillment, StringComparison.OrdinalIgnoreCase)))
                 );

            var fulfillmentComponent = cart.GetComponent<PhysicalFulfillmentComponent>();
            var p = context.GetPolicy<ShipEngine.Policies.ShipEnginePolicy>();

            // Exit if there is no physical fulfilment or adjustment
            if (defaultFulfillmentAdjustment == null || fulfillmentComponent == null || p == null || p.ShippingMethods.Count == 0)
            {
                return await Task.FromResult(cart);
            }

            var shippingMethod = p.ShippingMethods.Where(x => x.StartsWith(fulfillmentComponent.FulfillmentMethod.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

            if (shippingMethod == null)
            {
                return await Task.FromResult(cart);
            }
            else
            {
                shippingMethod = shippingMethod.Split('|')[1];
            }


            var shipEngine = new ShipEngineFulfillment();

            var shippingRateResult = shipEngine.GetShippingRate(shippingMethod, cart, context, _getSellableItemCommand);

            if (shippingRateResult == null)
            {
                shippingRateResult = new ShippingRateResult()
                { ErrorMessage = string.Format($"{0} - Unable to find shipping '{1}' rate (code='{2}') for product ''",
                            base.Name, fulfillmentComponent.FulfillmentMethod.Name, shippingMethod, Array.Empty<object>())
                };
            }

            if (shippingRateResult.HasError)
            {

                context.Abort(await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().Error, 
                    "InvalidOrMissingPropertyValue", 
                    new object[1] { shippingRateResult.ErrorMessage }
                )
                .ConfigureAwait(continueOnCapturedContext: false), context);

                return await Task.FromResult(cart);
            }

            // Replace Fulfillment adjustment overide the shipping value
            cart.Adjustments.Remove(defaultFulfillmentAdjustment);
            
            var awardedAdjustment = new CartLevelAwardedAdjustment
            {
                Name = defaultFulfillmentAdjustment.Name,
                DisplayName = defaultFulfillmentAdjustment.DisplayName,
                Adjustment = new Money(context.CommerceContext.CurrentCurrency(), (decimal)shippingRateResult.ShippingRate.Amount),
                AdjustmentType = context.GetPolicy<KnownCartAdjustmentTypesPolicy>().Fulfillment,
                AwardingBlock = base.Name,
                IsTaxable = defaultFulfillmentAdjustment.IsTaxable,
                IncludeInGrandTotal = defaultFulfillmentAdjustment.IncludeInGrandTotal
            };

            cart.Adjustments.Add(awardedAdjustment);

            context.Logger.LogInformation(string.Format("{0} - Adding cart fulfillment fee: {1} {2}"
                , base.Name, shippingRateResult.ShippingRate.Currency, shippingRateResult.ShippingRate.Amount), Array.Empty<object>());


            return await Task.FromResult(cart);
        }
    }
}
