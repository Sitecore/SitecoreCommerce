namespace Plugin.Sample.ExtendedConditions.EntityViews
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Framework.Rules;

    [EntityIdentifier("All items with a specific tag set price")]
    public class CartAllItemsWithTagSpecifyAmountAction : ICartLineAction
    {
        public void Execute(IRuleExecutionContext context)
        {
            var commerceContext = context.Fact<CommerceContext>();

            if (commerceContext == null)
            {
                return;
            }

            var cart = commerceContext.GetObject<Cart>();
            var totals = commerceContext.GetObject<CartTotals>();

            if ((cart != null) && cart.Lines.Any() && ((totals != null) && totals.Lines.Any()) == false)
            {
                return;
            }

            var source = new List<CartLineComponent>();
            foreach (var cartLine in cart.Lines.Where(x => x.HasComponent<CartProductComponent>()))
            {
                var firstOrDefault = cartLine.GetComponent<CartProductComponent>().Tags.FirstOrDefault(t => t.Name == this.Tag.Yield(context));
                if (!string.IsNullOrEmpty(firstOrDefault?.Name))
                {
                    source.Add(cartLine);
                }
            }

            if (!source.Any())
            {
                return;
            }

            var model = commerceContext.GetObject<PropertiesModel>();
            if (model == null)
            {
                return;
            }

            foreach (var line in source)
            {
                if (!totals.Lines.ContainsKey(line.Id))
                {
                    continue;
                }

                var discount = commerceContext.GetPolicy<KnownCartAdjustmentTypesPolicy>().Discount;
                var d = this.Price.Yield(context);
                if (commerceContext.GetPolicy<GlobalPricingPolicy>().ShouldRoundPriceCalc)
                {
                    d = decimal.Round(d, commerceContext.GetPolicy<GlobalPricingPolicy>().RoundDigits, commerceContext.GetPolicy<GlobalPricingPolicy>().MidPointRoundUp ? MidpointRounding.AwayFromZero : MidpointRounding.ToEven);
                }

                decimal amount;

                var currentAmount = totals.Lines[line.Id].SubTotal.Amount;
                if (currentAmount <= d)
                {
                    amount = d - currentAmount;
                    totals.Lines[line.Id].SubTotal.Amount += amount;
                }
                else
                {
                    amount = currentAmount - d;
                    amount = amount * decimal.MinusOne;
                    totals.Lines[line.Id].SubTotal.Amount += amount;
                }

                var item = new CartLineLevelAwardedAdjustment
                {
                    Name = (string)model.GetPropertyValue("PromotionText"),
                    DisplayName = (string)model.GetPropertyValue("PromotionCartText"),
                    Adjustment = new Money(commerceContext.CurrentCurrency(), amount),
                    AdjustmentType = discount,
                    IsTaxable = false,
                    AwardingBlock = "CartAllItemsWithTagSpecifyAmountAction"
                };
                line.Adjustments.Add(item);

                line.GetComponent<MessagesComponent>().AddMessage(commerceContext.GetPolicy<KnownMessageCodePolicy>().Promotions, $"PromotionApplied: {model.GetPropertyValue("PromotionId")}");
            }
        }

        public IRuleValue<decimal> Price { get; set; }

        public IRuleValue<string> Tag { get; set; }
    }
}