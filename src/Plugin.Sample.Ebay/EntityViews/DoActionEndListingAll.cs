namespace Plugin.Sample.Ebay.EntityViews
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Ebay.Commands;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionEndListingAll")]
    public class DoActionEndListingAll : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionEndListingAll(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("Ebay-EndListingAllItems", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var foundEntity = context.CommerceContext.GetObjects<FoundEntity>().FirstOrDefault(p => p.EntityId == entityView.EntityId);

                IEnumerable<SellableItem> ebayListedSellableItems = new List<SellableItem>();
                if (entityView.EntityId.Contains("Entity-Category-"))
                {
                    var category = foundEntity.Entity as Category;

                    var listName = $"{CatalogConstants.Relationships.CategoryToSellableItem}-{category.Id.SimplifyEntityName()}";

                    ebayListedSellableItems = await this._commerceCommander.Command<ListCommander>()
                        .GetListItems<SellableItem>(context.CommerceContext, listName, 0, 100);
                }
                else
                {
                    ebayListedSellableItems = await this._commerceCommander.Command<ListCommander>()
                        .GetListItems<SellableItem>(context.CommerceContext, "Ebay_Listed", 0, 100);
                }
                
                foreach (var listedItem in ebayListedSellableItems)
                {
                    await this._commerceCommander.Command<EbayCommand>().EndItemListing(context.CommerceContext, listedItem, "Incorrect");
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Ebay.DoActionEndItem.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "DoActionEndListingAll.Run.Exception", new object[] { ex }, ex.Message);
            }

            return entityView;
        }
    }
}
