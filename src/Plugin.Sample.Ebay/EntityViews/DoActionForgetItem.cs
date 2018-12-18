namespace Plugin.Sample.Ebay.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Ebay.Components;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionForgetItem")]
    public class DoActionForgetItem : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionForgetItem(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("Ebay-ForgetItem", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
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

                if (sellableItem.HasComponent<EbayItemComponent>())
                {
                    var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();
                    sellableItem.RemoveComponent(ebayItemComponent.GetType());
                }

                await this._commerceCommander.PersistEntity(context.CommerceContext, sellableItem);
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
