// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResolveSellableItemInventoryInformationBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.PerformanceTuning
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Inventory;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <inheritdoc />
    /// <summary>
    /// Defines the resolve sellable item inventory information
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Commerce.Core.PolicyTriggerConditionalPipelineBlock{Sitecore.Commerce.Plugin.Catalog.SellableItem,
    ///         Sitecore.Commerce.Plugin.Catalog.SellableItem}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(InventoryConstants.Pipelines.Blocks.ResolveSellableItemInventoryInformationBlock)]
    public class ResolveSellableItemInventoryInformationBlock : PolicyTriggerConditionalPipelineBlock<SellableItem, SellableItem>
    {
        private readonly GetCatalogCommand _getCatalogCommand;
        private readonly GetInventoryInformationCommand _getInventoryInformationCommand;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Commerce.Plugin.Inventory.ResolveSellableItemInventoryInformationBlock" /> class.
        /// </summary>
        /// <param name="getCatalogCommand">The get catalog command.</param>
        /// <param name="getInventoryInformationCommand">The get inventory information command.</param>
        public ResolveSellableItemInventoryInformationBlock(
            GetCatalogCommand getCatalogCommand,
            GetInventoryInformationCommand getInventoryInformationCommand)
        {
            this._getCatalogCommand = getCatalogCommand;
            this._getInventoryInformationCommand = getInventoryInformationCommand;
        }

        /// <inheritdoc />
        /// <summary>
        /// Gets the should not run policy trigger.
        /// </summary>
        /// <value>
        /// The should not run policy trigger.
        /// </value>
        public override string ShouldNotRunPolicyTrigger => "IgnoreInventory";

        /// <summary>
        /// Runs the specified argument.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>
        /// A <see cref="SellableItem" />
        /// </returns>
        public override async Task<SellableItem> Run(SellableItem arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The argument can not be null");

            var productArgument =
                context.CommerceContext.GetObject<ProductArgument>();

            if (productArgument == null)
            {
                return arg;
            }

            //context.CreateTraceReport();

            var catalog =
                context.CommerceContext.GetEntity<Catalog>(x =>
                    x.Name.Equals(
                        productArgument.CatalogName,
                        StringComparison.OrdinalIgnoreCase));

            if (catalog == null)
            {
                catalog =
                    await this._getCatalogCommand.Process(
                        context.CommerceContext,
                        productArgument.CatalogName);

                if (catalog == null)
                {
                    return arg;
                }

                context.CommerceContext.AddEntity(catalog);
            }

            var inventoryInformation = !string.IsNullOrEmpty(productArgument.VariantId)
                        ? context.CommerceContext.GetObjects<InventoryInformation>()
                            .FirstOrDefault(i =>
                                i.SellableItem.EntityTarget.Equals(arg.Id, StringComparison.OrdinalIgnoreCase) &&
                                i.VariationId.Equals(productArgument.VariantId, StringComparison.OrdinalIgnoreCase))
                        : context.CommerceContext.GetObjects<InventoryInformation>()
                            .FirstOrDefault(i =>
                                i.SellableItem.EntityTarget.Equals(arg.Id, StringComparison.OrdinalIgnoreCase));

            if (inventoryInformation == null)
            {
                await this._getInventoryInformationCommand.Process(
                context.CommerceContext,
                $"{CommerceEntity.IdPrefix<InventorySet>()}{catalog.DefaultInventorySetName}",
                arg.Id,
                productArgument.VariantId,
                true);
            }

            return arg;
        }
    }
}
