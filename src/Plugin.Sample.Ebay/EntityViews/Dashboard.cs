namespace Plugin.Sample.Ebay.EntityViews
{
    using System.Threading.Tasks;

    using global::Plugin.Sample.Ebay.Commands;
    using global::Plugin.Sample.Ebay.Components;
    using global::Plugin.Sample.Ebay.Entities;
    using global::Plugin.Sample.Ebay.Policies;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = global::Plugin.Sample.Ebay.Policies.PluginPolicy;

    [PipelineDisplayName("Dashboard")]
    public class Dashboard : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public Dashboard(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "MarketplacesDashboard")
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();
            var marketplaceDisplayPolicy = context.GetPolicy<MarketplaceDisplayPolicy>();
            var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(context.CommerceContext, "Entity-EbayConfigEntity-Global", true);
            if (!ebayConfig.IsPersisted)
            {
                ebayConfig.Id = "Entity-EbayConfigEntity-Global";
            }

            entityView.UiHint = "Flat";
            entityView.Icon = pluginPolicy.Icon;
            entityView.DisplayName = "Marketplaces";
            
            var ebayMarketplaceView = new EntityView
            {
                Name = "EbayMarketplace",
                DisplayName = "Ebay Marketplace",
                UiHint = "Flat",
                Icon = pluginPolicy.Icon,
                ItemId = string.Empty,
            };
            entityView.ChildViews.Add(ebayMarketplaceView);
            
            ebayMarketplaceView.Properties.Add(
                new ViewProperty
                {
                    Name = string.Empty,
                    IsHidden = false,
                    IsReadOnly = true,
                    OriginalType = "Html",
                    UiType = "Html",
                    RawValue = "<img alt='This is the alternate' height=50 width=100 src='https://www.paypalobjects.com/webstatic/en_AR/mktg/merchant/pages/sell-on-ebay/image-ebay.png' style=''/>"
                });

            var ebayGlobalConfigComponent = ebayConfig.GetComponent<EbayGlobalConfigComponent>();
            if (ebayGlobalConfigComponent != null)
            {
                ebayMarketplaceView.Properties.Add(
                    new ViewProperty
                    {
                        Name = "ReturnsPolicy",
                        DisplayName = "Returns Policy",
                        IsHidden = false,
                        IsReadOnly = true,
                        IsRequired = false,
                        RawValue = ebayGlobalConfigComponent.ReturnsPolicy
                    });
            }

            if (ebayConfig.HasComponent<EbayBusinessUserComponent>())
            {
                var ebayBusinessUserComponent = ebayConfig.GetComponent<EbayBusinessUserComponent>();
                if (!string.IsNullOrEmpty(ebayBusinessUserComponent.EbayToken))
                {
                    ebayMarketplaceView.Properties.Add(
                        new ViewProperty
                        {
                            Name = "TokenDate",
                            IsHidden = false,
                            IsReadOnly = true,
                            IsRequired = false,
                            RawValue = ebayBusinessUserComponent.TokenDate.ToString(marketplaceDisplayPolicy.DateTimeFormat)
                        });
                }

                var ebayTime = await this._commerceCommander.Command<EbayCommand>().GetEbayTime(context.CommerceContext);
                ebayMarketplaceView.Properties.Add(
                    new ViewProperty
                    {
                        Name = "EbayTime",
                        IsHidden = false,
                        IsReadOnly = true,
                        IsRequired = false,
                        RawValue = ebayTime
                    });
            }

            var ebayPendingSellableItems = await this._commerceCommander.Command<ListCommander>()
                .GetListItems<SellableItem>(context.CommerceContext, "Ebay_Pending", 0, 10);
            
            var ebayPendingSellableItemsView = new EntityView
            {
                EntityId = string.Empty,
                ItemId = "Id",
                DisplayName = "Pending Items",
                Name = "PendingItems",
                UiHint = "Table"
            };
            entityView.ChildViews.Add(ebayPendingSellableItemsView);

            foreach (var sellableItem in ebayPendingSellableItems)
            {
                var pathsView1 = new EntityView
                {
                    EntityId = string.Empty,
                    ItemId = sellableItem.Id,
                    DisplayName = "Sellable Item",
                    Name = "SellableItem",
                    UiHint = string.Empty
                };
                ebayPendingSellableItemsView.ChildViews.Add(pathsView1);

                pathsView1.Properties
                    .Add(new ViewProperty { Name = "Name", RawValue = sellableItem.Name, UiType = "EntityLink" });

                if (sellableItem.HasComponent<EbayItemComponent>())
                {
                    var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();
                    
                    pathsView1.Properties
                        .Add(new ViewProperty { Name = "Status", RawValue = ebayItemComponent.Status });
                    pathsView1.Properties
                        .Add(new ViewProperty { Name = "EbayId", RawValue = ebayItemComponent.EbayId });
                }
            }
            
            var listedItemsView = new EntityView
            {
                EntityId = string.Empty,
                ItemId = "Id",
                DisplayName = "Listed Items",
                Name = "ListedItems",
                UiHint = "Table"
            };
            entityView.ChildViews.Add(listedItemsView);

            var ebayListedSellableItems = await this._commerceCommander.Command<ListCommander>()
                .GetListItems<SellableItem>(context.CommerceContext, "Ebay_Listed", 0, 100);

            foreach (var sellableItem in ebayListedSellableItems)
            {
                var listedItemView = new EntityView
                {
                    EntityId = string.Empty,
                    ItemId = sellableItem.Id,
                    DisplayName = "Sellable Item",
                    Name = "SellableItem",
                    UiHint = string.Empty
                };
                listedItemsView.ChildViews.Add(listedItemView);

                listedItemView.Properties
                    .Add(new ViewProperty { Name = "Name", RawValue = sellableItem.Name, UiType = "EntityLink" });

                if (sellableItem.HasComponent<EbayItemComponent>())
                {
                    var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();
                    listedItemView.Properties
                        .Add(new ViewProperty { Name = "Status", RawValue = ebayItemComponent.Status });
                    listedItemView.Properties
                        .Add(new ViewProperty { Name = "EbayId", RawValue = ebayItemComponent.EbayId });
                }
            }

            return entityView;
        }
    }
}
