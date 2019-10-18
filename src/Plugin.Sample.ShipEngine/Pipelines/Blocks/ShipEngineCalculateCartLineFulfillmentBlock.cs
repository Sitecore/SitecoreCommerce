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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.ShipEngine.Pipelines.Blocks
{
    public class ShipEngineCalculateCartLineFulfillmentBlock : PipelineBlock<Cart, Cart, CommercePipelineExecutionContext>
    {


        private readonly GetSellableItemCommand _getSellableItemCommand;

        public ShipEngineCalculateCartLineFulfillmentBlock(GetSellableItemCommand getSellableItemCommand)
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
            Condition.Requires<IList<CartLineComponent>>(cart.Lines).IsNotNull<IList<CartLineComponent>>(base.Name + ": The cart lines cannot be null.");

            // If the cart is not null and has product lines
            if (cart == null || !cart.Lines.Any())
            {
                return await Task.FromResult(cart);
            }

            var fulfilmentCartLines = cart.Lines.Where<CartLineComponent>(cl => cl != null && cl.HasComponent<PhysicalFulfillmentComponent>());

            if (!fulfilmentCartLines.Any())
            {
                context.Logger.LogInformation(base.Name + " - No lines with fulfillment components", Array.Empty<object>());
                return await Task.FromResult<Cart>(cart);
            }


            // loop through to reset Shipping for all line that already has fulfilment set.
            foreach (var cartLineComponent in fulfilmentCartLines)
            {

                // Check to see if there is existing fulfillment fee block to replace
                var defaultFulfillmentAdjustment = cartLineComponent.Adjustments.FirstOrDefault(a =>
                       (a.Name.Equals("FulfillmentFee", StringComparison.OrdinalIgnoreCase)
                       && (!string.IsNullOrEmpty(a.AdjustmentType)
                       && a.AdjustmentType.Equals(context.GetPolicy<KnownCartAdjustmentTypesPolicy>().Fulfillment, StringComparison.OrdinalIgnoreCase)))
                     );

                var fulfillmentComponent = cartLineComponent.GetComponent<PhysicalFulfillmentComponent>();
                var p = context.GetPolicy<ShipEngine.Policies.ShipEnginePolicy>();

                // Exit if there is no physical fulfilment or adjustment for line item
                if (defaultFulfillmentAdjustment == null || fulfillmentComponent == null || p == null || p.ShippingMethods.Count == 0)
                {
                    continue;
                }

                var shippingMethod = p.ShippingMethods.Where(x => x.StartsWith(fulfillmentComponent.FulfillmentMethod.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();

                if (shippingMethod == null)
                {
                    continue;
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
                    {
                        ErrorMessage = string.Format($"{0} - Unable to find shipping '{1}' rate (code='{2}') for product ''",
                                base.Name, fulfillmentComponent.FulfillmentMethod.Name, shippingMethod, Array.Empty<object>())
                    };
                }

                if (shippingRateResult.HasError)
                {
                    context.Logger.LogCritical( shippingRateResult.ErrorMessage, Array.Empty<object>());

                    if (!shippingRateResult.IsErrorAddedToCommerceContext)
                    {
                       await context.CommerceContext.AddMessage(
                            context.GetPolicy<KnownResultCodes>().Error,
                            "InvalidOrMissingPropertyValue",
                            new object[1] { shippingRateResult.ErrorMessage }
                             ).ConfigureAwait(continueOnCapturedContext: false);
                    }

                    continue;
                }

                // Replace Fulfillment adjustment overide the shipping value
                cartLineComponent.Adjustments.Remove(defaultFulfillmentAdjustment);
                var awardedAdjustment = new CartLevelAwardedAdjustment
                {
                    Name = defaultFulfillmentAdjustment.Name,
                    DisplayName = defaultFulfillmentAdjustment.DisplayName,
                    Adjustment = new Money(context.CommerceContext.CurrentCurrency(),
                            (decimal)shippingRateResult.ShippingRate.Amount
                            ),
                    AdjustmentType = context.GetPolicy<KnownCartAdjustmentTypesPolicy>().Fulfillment,
                    AwardingBlock = base.Name,
                    IsTaxable = defaultFulfillmentAdjustment.IsTaxable,
                    IncludeInGrandTotal = defaultFulfillmentAdjustment.IncludeInGrandTotal
                };

                cartLineComponent.Adjustments.Add(awardedAdjustment);

                context.Logger.LogInformation($"{base.Name} - Adding cartLine fulfillment fee to line: {shippingRateResult.ShippingRate.Currency} {shippingRateResult.ShippingRate.Amount}", Array.Empty<object>());
            }


            return await Task.FromResult(cart);
        }
    }
}
