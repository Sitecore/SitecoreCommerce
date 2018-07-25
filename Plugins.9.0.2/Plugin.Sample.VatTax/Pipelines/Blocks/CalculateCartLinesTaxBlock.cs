// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalculateCartLinesTaxBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.VatTax
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Fulfillment;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Commerce.Plugin.Tax;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which calculates the Cart lines tax totals.
    /// This is intended to be a SAMPLE Tax calculator and not a production supported one.
    /// Customers will normally replace this block with a call out to a third party tax calculator.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Plugin.Carts.Cart,
    ///         Sitecore.Commerce.Plugin.Carts.Cart, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("CalculateCartLinesTax")]
    public class CalculateCartLinesTaxBlock : PipelineBlock<Cart, Cart, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistCustomerBlock"/> class.
        /// </summary>
        /// <param name="commander">
        /// Invokes the CommerceCommander.
        /// </param>
        public CalculateCartLinesTaxBlock(CommerceCommander commander)
        {
            this._commerceCommander = commander;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="arg">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Cart"/>.
        /// </returns>
        public override async Task<Cart> Run(Cart arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: The cart can not be null");
            Condition.Requires(arg.Lines).IsNotNull($"{this.Name}: The cart lines can not be null");

            if (!arg.Lines.Any())
            {
                return arg;
            }

            //var lines = arg.Lines.Where(line => line != null && line.HasComponent<FulfillmentComponent>()).Select(l => l).ToList();
            //if (!lines.Any())
            //{
            //    context.Logger.LogDebug($"{this.Name} - No lines to calculate tax on");
            //    return Task.FromResult(arg);
            //}

            var currency = context.CommerceContext.CurrentCurrency();

            var globalTaxPolicy = context.GetPolicy<GlobalTaxPolicy>();
            var globalPricingPolicy = context.GetPolicy<GlobalPricingPolicy>();
            context.Logger.LogDebug($"{this.Name} - Policy:{globalTaxPolicy.TaxCalculationEnabled}");

            var vatTaxTable = await this._commerceCommander.Command<ListCommander>()
                        .GetListItems<VatTaxTableEntity>(context.CommerceContext,
                        CommerceEntity.ListName<VatTaxTableEntity>(), 0, 99);

            foreach (var line in arg.Lines)
            {
                var lineProductComponent = line.GetComponent<CartProductComponent>();
                var tags = lineProductComponent.Tags;
                foreach(var taxRateLine in vatTaxTable)
                {
                    if (tags.Any(p=>p.Name == taxRateLine.TaxTag)){
                        if (arg.HasComponent <PhysicalFulfillmentComponent>()){
                            var physicalFulfillment = arg.GetComponent<PhysicalFulfillmentComponent>();
                            var party = physicalFulfillment.ShippingParty;
                            var country = party.CountryCode;

                            if (taxRateLine.CountryCode == country)
                            {
                                var taxRate = taxRateLine.TaxPct / 100;
                                context.Logger.LogDebug($"{this.Name} - Item Tax Rate:{taxRate}");
                                // if we are allowing tax exemptions, check the cart product for the tax exempt tag
                                //if (globalTaxPolicy.TaxExemptTagsEnabled &&
                                //    line.HasComponent<CartProductComponent>() &&
                                //    line.GetComponent<CartProductComponent>().Tags.Select(t => t.Name).Contains(globalTaxPolicy.TaxExemptTag, StringComparer.InvariantCultureIgnoreCase))
                                //{
                                //    context.Logger.LogDebug($"{this.Name} - Skipping Tax Calculation for product {line.ItemId} due to exempt tag");
                                //    continue;
                                //}

                                var adjustmentTotal = line.Adjustments.Where(a => a.IsTaxable).Aggregate(0M, (current, adjustment) => current + adjustment.Adjustment.Amount);

                                context.Logger.LogDebug($"{this.Name} - SubTotal:{line.Totals.SubTotal.Amount}");
                                context.Logger.LogDebug($"{this.Name} - Adjustment Total:{adjustmentTotal}");

                                var tax = new Money(currency, (line.Totals.SubTotal.Amount + adjustmentTotal) * taxRate);

                                if (globalPricingPolicy.ShouldRoundPriceCalc)
                                {
                                    tax.Amount = decimal.Round(
                                        tax.Amount,
                                        globalPricingPolicy.RoundDigits,
                                        globalPricingPolicy.MidPointRoundUp ? MidpointRounding.AwayFromZero : MidpointRounding.ToEven);
                                }

                                line.Adjustments.Add(new CartLineLevelAwardedAdjustment
                                {
                                    Name = TaxConstants.TaxAdjustmentName,
                                    DisplayName = $"{taxRate * 100}% Vat Tax",
                                    Adjustment = tax,
                                    AdjustmentType = context.GetPolicy<KnownCartAdjustmentTypesPolicy>().Tax,
                                    AwardingBlock = this.Name,
                                    IsTaxable = false,
                                    IncludeInGrandTotal = false
                                });
                            }
                        }
                    }
                }
                
            }

            return arg;
        }
    }
}
