namespace Plugin.Sample.Ebay.EntityViews
{
    using System.Linq;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Ebay.Components;
    using global::Plugin.Sample.Ebay.Entities;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("EnsureActions")]
    public class EnsureActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public EnsureActions(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");
            
            if (entityView.Name != "Master")
            {
                return entityView;
            }
            if (!entityView.EntityId.Contains("Entity-SellableItem-"))
            {
                return entityView;
            }

            var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(context.CommerceContext, "Entity-EbayConfigEntity-Global", true);

            if (ebayConfig.HasComponent<EbayBusinessUserComponent>())
            {
                var ebayConfigComponent = ebayConfig.GetComponent<EbayBusinessUserComponent>();
                if (!string.IsNullOrEmpty(ebayConfigComponent.EbayToken))
                {
                    var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

                    if (entityViewArgument.Entity is SellableItem sellableItem)
                    {
                        if (sellableItem.HasComponent<EbayItemComponent>())
                        {
                            var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();

                            var ebayView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Ebay Marketplace Item");
                            if (ebayView != null)
                            {
                                if (ebayItemComponent.Status == "Listed")
                                {
                                    ebayView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                                    {
                                        Name = "Ebay-EndItem",
                                        DisplayName = $"End Listing On Ebay",
                                        Description = string.Empty,
                                        IsEnabled = true,
                                        RequiresConfirmation = false,
                                        EntityView = "Ebay-FormEndItem",
                                        UiHint = string.Empty,
                                        Icon = "shelf_empty"
                                    });
                                }
                                else if (ebayItemComponent.Status == "Pending")
                                {
                                    ebayView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                                    {
                                        Name = "Ebay-EndItem",
                                        DisplayName = $"End Listing On Ebay",
                                        Description = string.Empty,
                                        IsEnabled = true,
                                        RequiresConfirmation = false,
                                        EntityView = "Ebay-FormEndItem",
                                        UiHint = string.Empty,
                                        Icon = "shelf_empty"
                                    });
                                }
                                else if (ebayItemComponent.Status == "LostSync")
                                {
                                    ebayView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                                    {
                                        Name = "Ebay-FixSyncItem",
                                        DisplayName = $"Fix Sync of Ebay Item",
                                        Description = string.Empty,
                                        IsEnabled = true,
                                        RequiresConfirmation = true,
                                        EntityView = string.Empty,
                                        UiHint = string.Empty,
                                        Icon = "wrench"
                                    });
                                }
                                else
                                {
                                    ebayView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                                    {
                                        Name = "Ebay-StartSelling",
                                        DisplayName = $"List this Item On Ebay",
                                        Description = string.Empty,
                                        IsEnabled = true,
                                        RequiresConfirmation = false,
                                        EntityView = "Ebay-FormStartSelling",
                                        UiHint = string.Empty,
                                        Icon = "shelf_full"
                                    });
                                    if (ebayItemComponent.Status == "Ended")
                                    {
                                        ebayView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                                        {
                                            Name = "Ebay-ForgetItem",
                                            DisplayName = $"Forget Ebay Component from this Item",
                                            Description = string.Empty,
                                            IsEnabled = true,
                                            RequiresConfirmation = true,
                                            EntityView = string.Empty,
                                            UiHint = string.Empty,
                                            Icon = "shelf_full"
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            entityView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                            {
                                Name = "Ebay-StartSelling",
                                DisplayName = $"List this Item On Ebay",
                                Description = string.Empty,
                                IsEnabled = true,
                                RequiresConfirmation = false,
                                EntityView = "Ebay-FormStartSelling",
                                UiHint = string.Empty,
                                Icon = "shelf_full"
                            });
                        }
                    }
                }
            }

            return entityView;
        }
    }
}
