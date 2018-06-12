// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CalculateCartLinesPriceBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.PerformanceTuning
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which calculates a cart lines price.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Plugin.Carts.Cart,
    ///         Sitecore.Commerce.Plugin.Carts.Cart, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(CatalogConstants.Pipelines.Blocks.CalculateCartLinesPriceBlock)]
    public class CalculateCartLinesPriceBlock : PipelineBlock<Cart, Cart, CommercePipelineExecutionContext>
    {
        private readonly GetSellableItemCommand _getSellableItemCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculateCartLinesPriceBlock"/> class.
        /// </summary>
        /// <param name="getSellableItemCommand">The get sellable item command.</param>
        public CalculateCartLinesPriceBlock(GetSellableItemCommand getSellableItemCommand)
        {
            this._getSellableItemCommand = getSellableItemCommand;
        }

        /// <summary>
        /// Runs with the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>A <see cref="Cart"/></returns>
        public override async Task<Cart> Run(Cart arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: Cart cannot be null.");
            Condition.Requires(arg.Lines).IsNotNull($"{this.Name}: The cart's lines cannot be null");

            if (!arg.Lines.Any())
            {
                return arg;
            }

            foreach (var line in arg.Lines.Where(l => l != null && l.HasComponent<CartProductComponent>() && !string.IsNullOrEmpty(l.ItemId) && l.ItemId.Split('|').Length >= 2))
            {
                var lineVariant = line.ChildComponents.OfType<ItemVariationSelectedComponent>().FirstOrDefault();

                if (line.HasPolicy<PurchaseOptionMoneyPolicy>())
                {
                    //Skip if we have already calculated pricing
                    continue;
                }

                //Check Object collection first
                var cartProductComponent = line.GetComponent<CartProductComponent>();

                var sellableItem = context.CommerceContext.GetObjects<SellableItem>().FirstOrDefault(p => p.Id == $"Entity-SellableItem-{cartProductComponent.Id}");
                if (sellableItem == null)
                {
                    sellableItem = await this._getSellableItemCommand.Process(context.CommerceContext, line.ItemId, false);
                    if (sellableItem == null)
                    {
                        context.Logger.LogError($"{this.Name}-SellableItemNotFound for Cart Line: ItemId={line.ItemId}|CartId={arg.Id}|LineId={line.Id}");
                        return arg;
                    }
                }

                // CLEANING PRICING MESSAGES
                var messagesComponent = line.GetComponent<MessagesComponent>();
                messagesComponent.Clear(context.GetPolicy<KnownMessageCodePolicy>().Pricing);

                if (sellableItem.HasComponent<MessagesComponent>())
                {
                    var pricingMessages = sellableItem.GetComponent<MessagesComponent>().GetMessages(context.GetPolicy<KnownMessageCodePolicy>().Pricing);
                    messagesComponent.AddMessages(pricingMessages);
                }

                line.UnitListPrice = sellableItem.ListPrice;
                var listPriceMessage = $"CartItem.ListPrice<=SellableItem.ListPrice: Price={line.UnitListPrice.AsCurrency()}";

                var sellPriceMessage = string.Empty;
                var purchaseOptionPolicy = new PurchaseOptionMoneyPolicy();
                if (sellableItem.HasPolicy<PurchaseOptionMoneyPolicy>())
                {
                    purchaseOptionPolicy.SellPrice = sellableItem.GetPolicy<PurchaseOptionMoneyPolicy>().SellPrice;
                    sellPriceMessage = $"CartItem.SellPrice<=SellableItem.SellPrice: Price={purchaseOptionPolicy.SellPrice.AsCurrency()}";
                }

                PriceSnapshotComponent snapshot;
                if (sellableItem.HasComponent<ItemVariationsComponent>())
                {
                    var variation = sellableItem.GetComponent<ItemVariationsComponent>()?.ChildComponents?.OfType<ItemVariationComponent>()
                            .FirstOrDefault(v => !string.IsNullOrEmpty(v.Id) && v.Id.Equals(lineVariant?.VariationId, StringComparison.OrdinalIgnoreCase));
                    if (variation != null)
                    {
                        if (variation.HasComponent<MessagesComponent>())
                        {
                            var pricingMessages = variation.GetComponent<MessagesComponent>().GetMessages(context.GetPolicy<KnownMessageCodePolicy>().Pricing);
                            messagesComponent.AddMessages(pricingMessages);
                        }

                        line.UnitListPrice = variation.ListPrice;
                        listPriceMessage = $"CartItem.ListPrice<=SellableItem.Variation.ListPrice: Price={line.UnitListPrice.AsCurrency()}";

                        if (variation.HasPolicy<PurchaseOptionMoneyPolicy>())
                        {
                            purchaseOptionPolicy.SellPrice = variation.GetPolicy<PurchaseOptionMoneyPolicy>().SellPrice;
                            sellPriceMessage = $"CartItem.SellPrice<=SellableItem.Variation.SellPrice: Price={purchaseOptionPolicy.SellPrice.AsCurrency()}";
                        }
                    }

                    snapshot = variation?.ChildComponents.OfType<PriceSnapshotComponent>().FirstOrDefault();
                }
                else
                {
                    snapshot = sellableItem.Components.OfType<PriceSnapshotComponent>().FirstOrDefault();
                }

                var tier = snapshot?.Tiers.OrderByDescending(t => t.Quantity).FirstOrDefault(t => t.Quantity <= line.Quantity);
                if (tier != null)
                {
                    purchaseOptionPolicy.SellPrice = new Money(tier.Currency, tier.Price);
                    sellPriceMessage = $"CartItem.SellPrice<=PriceCard.ActiveSnapshot: Price={purchaseOptionPolicy.SellPrice.AsCurrency()}|Qty={tier.Quantity}";
                }

                line.Policies.Remove(line.Policies.OfType<PurchaseOptionMoneyPolicy>().FirstOrDefault());
                if (purchaseOptionPolicy.SellPrice == null)
                {
                    continue;
                }

                line.SetPolicy(purchaseOptionPolicy);
                if (!string.IsNullOrEmpty(sellPriceMessage))
                {
                    messagesComponent.AddMessage(context.GetPolicy<KnownMessageCodePolicy>().Pricing, sellPriceMessage);
                }

                if (!string.IsNullOrEmpty(listPriceMessage))
                {
                    messagesComponent.AddMessage(context.GetPolicy<KnownMessageCodePolicy>().Pricing, listPriceMessage);
                }
            }

            return arg;
        }
    }
}
