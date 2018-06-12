// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopulateLineItemInventoryBlock.cs" company="Sitecore Corporation">
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
    using Sitecore.Commerce.Plugin.Availability;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Inventory;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which populates a line with inventory information.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.Plugin.Carts.CartLineComponent,
    ///         Sitecore.Commerce.Plugin.Carts.CartLineComponent, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(InventoryConstants.Pipelines.Blocks.PopulateLineItemInventoryBlock)]
    public class PopulateLineItemInventoryBlock : PipelineBlock<CartLineComponent, CartLineComponent, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="arg">
        /// The line to process.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="CartLineComponent"/>.
        /// </returns>
        public override Task<CartLineComponent> Run(CartLineComponent arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: The line can not be null");

            if (arg.HasComponent<ItemAvailabilityComponent>())
            {
                var availabilityComponent = arg.GetComponent<ItemAvailabilityComponent>();
                context.Logger.LogDebug($"{this.Name} - ItemAvailabilityComponent already exists - Expires: {availabilityComponent.AvailabilityExpires}");
                if (availabilityComponent.AvailabilityExpires >= context.CommerceContext.CurrentEffectiveDate())
                {
                    context.Logger.LogDebug($"{this.Name} - ItemAvailabilityComponent already exists - Not Expired!: {availabilityComponent.AvailabilityExpires}");
                    return Task.FromResult(arg);
                }

                context.Logger.LogInformation($"{this.Name}.ItemAvailabilityComponent.Expired: EffectiveDate={context.CommerceContext.CurrentEffectiveDate()}|Expires={availabilityComponent.AvailabilityExpires}|ItemId={arg.ItemId}");
            }

            var cartProductComponent = arg.GetComponent<CartProductComponent>();
            if (cartProductComponent.HasPolicy<AvailabilityAlwaysPolicy>())
            {
                if (arg.HasComponent<ItemAvailabilityComponent>())
                {
                    arg.ChildComponents.Remove(arg.ChildComponents.OfType<ItemAvailabilityComponent>().First());
                }

                return Task.FromResult(arg);
            }

            var productArgument = ProductArgument.FromItemId(arg.ItemId);
            if (!productArgument.IsValid())
            {
                context.Logger.LogError($"{this.Name}-ProductArgument Invalid: ItemId={arg.ItemId}");
                return Task.FromResult(arg);
            }

            var sellableItem = context.CommerceContext.GetEntity<SellableItem>(s => s.FriendlyId.Equals(productArgument.ProductId, StringComparison.OrdinalIgnoreCase));
            if (sellableItem == null)
            {
                return Task.FromResult(arg);
            }

            if (sellableItem.HasComponent<ItemVariationsComponent>())
            {
                var variation = sellableItem.GetComponent<ItemVariationsComponent>().GetComponents<ItemVariationComponent>().FirstOrDefault(v => v.Id.Equals(productArgument.VariantId, StringComparison.OrdinalIgnoreCase));
                if (variation != null && variation.HasComponent<ItemAvailabilityComponent>())
                {
                    var availability = variation.GetComponent<ItemAvailabilityComponent>();
                    availability.ItemId = arg.ItemId;
                    arg.SetComponent(availability);
                }
            }
            else if (sellableItem.HasComponent<ItemAvailabilityComponent>())
            {
                var availability = sellableItem.GetComponent<ItemAvailabilityComponent>();
                availability.ItemId = arg.ItemId;
                arg.SetComponent(availability);
            }

            context.Logger.LogInformation($"PopulateLineItemAvailability.ComponentUpdated.{arg.GetComponent<ItemAvailabilityComponent>().ItemId}: Count={arg.GetComponent<ItemAvailabilityComponent>().AvailableQuantity}");

            return Task.FromResult(arg);
        }
    }
}
