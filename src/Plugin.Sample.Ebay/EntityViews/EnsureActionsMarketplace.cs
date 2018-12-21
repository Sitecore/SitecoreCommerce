namespace Plugin.Sample.Ebay.EntityViews
{
    using System.Linq;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Ebay.Components;
    using global::Plugin.Sample.Ebay.Entities;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("EnsureActionsMarketplace")]
    public class EnsureActionsMarketplace : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public EnsureActionsMarketplace(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");
            
            if (entityView.Name == "MarketplacesDashboard")
            {
                var ebayView = entityView.ChildViews.FirstOrDefault(p => p.Name == "EbayMarketplace");
                if (ebayView != null)
                {
                    var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(context.CommerceContext, "Entity-EbayConfigEntity-Global", true);

                    if (ebayConfig.HasComponent<EbayBusinessUserComponent>())
                    {
                        var ebayBusinessUserComponent = ebayConfig.GetComponent<EbayBusinessUserComponent>();
                        if (!string.IsNullOrEmpty(ebayBusinessUserComponent.EbayToken))
                        {
                            ebayView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                            {
                                Name = "Ebay-Configure",
                                DisplayName = $"Configure Ebay",
                                Description = string.Empty,
                                IsEnabled = true,
                                RequiresConfirmation = false,
                                EntityView = "Ebay-FormConfigure",
                                UiHint = string.Empty,
                                Icon = "control_panel"
                            });

                            ebayView.GetPolicy<ActionsPolicy>().Actions.Add(
                                new EntityActionView
                                {
                                    Name = "Ebay-RemoveToken",
                                    DisplayName = $"Remove Ebay User Token",
                                    Description = string.Empty,
                                    IsEnabled = true,
                                    RequiresConfirmation = true,
                                    EntityView = string.Empty,
                                    UiHint = string.Empty,
                                    Icon = "barrier_open"
                                });

                            var listedItemsView = entityView.ChildViews.FirstOrDefault(p => p.Name == "ListedItems");
                            if (listedItemsView != null)
                            {
                                listedItemsView.GetPolicy<ActionsPolicy>().Actions.Add(
                                    new EntityActionView
                                    {
                                        Name = "Ebay-EndItem",
                                        DisplayName = $"End Item Listing on Ebay",
                                        Description = string.Empty,
                                        IsEnabled = true,
                                        RequiresConfirmation = false,
                                        EntityView = "Ebay-FormEndItem",
                                        UiHint = string.Empty,
                                        Icon = "shelf_empty"
                                    });

                                listedItemsView.GetPolicy<ActionsPolicy>().Actions.Add(
                                    new EntityActionView
                                    {
                                        Name = "Ebay-EndListingAllItems",
                                        DisplayName = $"End All Item Listings on Ebay",
                                        Description = string.Empty,
                                        IsEnabled = true,
                                        RequiresConfirmation = true,
                                        EntityView = string.Empty,
                                        UiHint = string.Empty,
                                        Icon = "shelf_empty"
                                    });
                            }

                            var pendingItemsView = entityView.ChildViews.FirstOrDefault(p => p.Name == "PendingItems");
                            pendingItemsView?.GetPolicy<ActionsPolicy>().Actions.Add(
                                new EntityActionView
                                {
                                    Name = "Ebay-PublishPending",
                                    DisplayName = $"Publish Pending Listings to Ebay",
                                    Description = string.Empty,
                                    IsEnabled = true,
                                    RequiresConfirmation = true,
                                    EntityView = string.Empty,
                                    UiHint = string.Empty,
                                    Icon = "shelf_full"
                                });
                        }
                        else
                        {
                            ebayView.GetPolicy<ActionsPolicy>().Actions.Add(
                                new EntityActionView
                                {
                                    Name = "Ebay-RegisterToken",
                                    DisplayName = $"Register an Ebay User Token",
                                    Description = string.Empty,
                                    IsEnabled = true,
                                    RequiresConfirmation = false,
                                    EntityView = "Ebay-FormRegisterToken",
                                    UiHint = string.Empty,
                                    Icon = "barrier_closed"
                                });
                        }
                    }
                    else
                    {
                        ebayView.GetPolicy<ActionsPolicy>().Actions.Add(
                            new EntityActionView
                            {
                                Name = "Ebay-RegisterToken",
                                DisplayName = $"Register an Ebay User Token",
                                Description = string.Empty,
                                IsEnabled = true,
                                RequiresConfirmation = false,
                                EntityView = "Ebay-FormRegisterToken",
                                UiHint = string.Empty,
                                Icon = "barrier_closed"
                            });
                    }
                }

                return entityView;
            }

            return entityView;
        }
    }
}
