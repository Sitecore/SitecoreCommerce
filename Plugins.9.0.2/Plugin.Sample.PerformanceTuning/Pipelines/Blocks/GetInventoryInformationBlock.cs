// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetInventoryInformationBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.PerformanceTuning
{
    using System.Linq;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
    using Sitecore.Commerce.Plugin.Inventory;

    /// <summary>
    /// Defines the create inventory set block.
    /// </summary>
    [PipelineDisplayName(InventoryConstants.Pipelines.Blocks.GetInventoryInformationBlock)]
    public class GetInventoryInformationBlock : PipelineBlock<SellableItemInventorySetArgument, InventoryInformation, CommercePipelineExecutionContext>
    {
        private readonly IFindEntityPipeline _findEntityPipeline;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetInventoryInformationBlock" /> class.
        /// </summary>
        /// <param name="findEntityPipeline">The find entity pipeline.</param>
        public GetInventoryInformationBlock(IFindEntityPipeline findEntityPipeline)
        {
            this._findEntityPipeline = findEntityPipeline;
        }

        /// <summary>
        /// Executes the pipeline block.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public override async Task<InventoryInformation> Run(
            SellableItemInventorySetArgument arg,
            CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The argument can not be null");

            //Check in Entities collection
            var sellableItem = context.CommerceContext
                .GetEntity<SellableItem>(x => x.Id.Equals(arg.SellableItemId, 
                StringComparison.OrdinalIgnoreCase));

            //Check in Objects collection
            if (sellableItem == null)
            {
                sellableItem = context.CommerceContext
                    .GetObjects<SellableItem>()
                    .FirstOrDefault(p => p.Id == arg.SellableItemId);
            }

            //Load from persistance
            if (sellableItem == null)
            {
                sellableItem = await this._findEntityPipeline
                    .Run(new FindEntityArgument(typeof(SellableItem), arg.SellableItemId
                    .EnsurePrefix(CommerceEntity.IdPrefix<SellableItem>())), context) as SellableItem;
            }
            
            if (sellableItem == null)
            {
                return null;
            }

            var inventoryComponent = sellableItem.GetComponent<InventoryComponent>(arg.VariationId);

            var inventoryAssociation = inventoryComponent.InventoryAssociations.FirstOrDefault(x =>
                    x.InventorySet.EntityTarget.Equals(arg.InventorySetId.EnsurePrefix(CommerceEntity.IdPrefix<InventorySet>()), StringComparison.OrdinalIgnoreCase));
            if (inventoryAssociation == null)
            {
                return null;
            }

            // Inventory is NOT cached intentionally, because it might result in concurrency issues when multiple checkouts (or inventory updates in general) happen at the same time.
            var inventoryInformation = await this._findEntityPipeline.Run(
                    new FindEntityArgument(typeof(InventoryInformation), inventoryAssociation.InventoryInformation.EntityTarget), context) as InventoryInformation;

            context.CommerceContext.AddUniqueEntity(inventoryInformation);

            return inventoryInformation;
        }
    }
}