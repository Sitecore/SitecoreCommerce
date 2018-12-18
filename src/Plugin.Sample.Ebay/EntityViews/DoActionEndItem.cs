namespace Plugin.Sample.Ebay.EntityViews
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Ebay.Commands;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionEndItem")]
    public class DoActionEndItem : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionEndItem(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("Ebay-EndItem", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var reason = entityView.Properties.First(p => p.Name == "Reason").Value ?? "";

                var sellableItemId = entityView.EntityId;
                if (string.IsNullOrEmpty(sellableItemId))
                {
                    sellableItemId = entityView.ItemId;
                }
                if (sellableItemId.Contains("Entity-Category-"))
                {
                    sellableItemId = entityView.ItemId;
                }
                var sellableItem = await this._commerceCommander.GetEntity<SellableItem>(context.CommerceContext, sellableItemId);

                await this._commerceCommander.Command<EbayCommand>().EndItemListing(context.CommerceContext, sellableItem, reason);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Ebay.DoActionEndItem.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "DocActionEndItem.Exception", new object[] { ex }, ex.Message);
            }

            return entityView;
        }
    }
}
