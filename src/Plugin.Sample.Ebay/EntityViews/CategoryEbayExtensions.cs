namespace Plugin.Sample.Ebay.EntityViews
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Ebay.Components;
    using global::Plugin.Sample.Ebay.Entities;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("CategoryEbayExtensions")]
    public class CategoryEbayExtensions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public CategoryEbayExtensions(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            try
            {
                if (entityView.EntityId.Contains("Entity-Category-"))
                {
                    var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(context.CommerceContext, "Entity-EbayConfigEntity-Global", true);

                    if (ebayConfig.HasComponent<EbayBusinessUserComponent>())
                    {
                        if (entityView.ChildViews.FirstOrDefault(p => p.Name == "SellableItems") is EntityView sellableItemsView)
                        {
                            foreach (var sellableItemViewItem in sellableItemsView.ChildViews)
                            {
                                var sellableItemViewItemAsEntityView = sellableItemViewItem as EntityView;

                                var sellableItemId = (sellableItemViewItemAsEntityView).ItemId;
                                var foundEntity = context.CommerceContext.GetObjects<FoundEntity>().FirstOrDefault(p => p.EntityId == sellableItemId);
                                if (foundEntity != null)
                                {
                                    var sellableItem = foundEntity.Entity as SellableItem;
                                    if (sellableItem.HasComponent<EbayItemComponent>())
                                    {
                                        var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();

                                        if (ebayItemComponent.Status != "Ended")
                                        {
                                            sellableItemViewItemAsEntityView.Properties.Insert(0,
                                                new ViewProperty
                                                {
                                                    Name = "Marketplaces",
                                                    IsHidden = false,
                                                    IsReadOnly = true,
                                                    OriginalType = "Html",
                                                    UiType = "Html",
                                                    RawValue = "<img alt='This is the alternate' height=20 width=40 src='https://www.paypalobjects.com/webstatic/en_AR/mktg/merchant/pages/sell-on-ebay/image-ebay.png' style=''/>"
                                                });
                                        }
                                        else
                                        {
                                            sellableItemViewItemAsEntityView.Properties.Insert(0,
                                                new ViewProperty
                                                {
                                                    Name = string.Empty,
                                                    IsHidden = false,
                                                    IsReadOnly = true,
                                                    UiType = string.Empty,
                                                    RawValue = string.Empty
                                                });
                                        }
                                    }
                                    else
                                    {
                                        sellableItemViewItemAsEntityView.Properties.Insert(0,
                                            new ViewProperty
                                            {
                                                Name = string.Empty,
                                                IsHidden = false,
                                                IsReadOnly = true,
                                                UiType = string.Empty,
                                                RawValue = string.Empty
                                            });
                                    }
                                }
                            }
                        }

                        return entityView;
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Ebay.CategoryEbayExtensions.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "CategoryEbayExtensions.Run.Exception", new object[] { ex }, ex.Message);
            }

            return entityView;
        }
    }
}
