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
   
    [PipelineDisplayName("EnsureActionsCategory")]
    public class EnsureActionsCategory : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public EnsureActionsCategory(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");
            
            if (entityView.EntityId.Contains("Entity-Category-"))
            {
                var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(context.CommerceContext, "Entity-EbayConfigEntity-Global", true);

                if (ebayConfig.HasComponent<EbayBusinessUserComponent>())
                {
                    var ebayConfigComponent = ebayConfig.GetComponent<EbayBusinessUserComponent>();
                    if (!string.IsNullOrEmpty(ebayConfigComponent.EbayToken))
                    {
                        if (entityView.ChildViews.FirstOrDefault(p => p.Name == "SellableItems") is EntityView sellableItemsView)
                        {
                            sellableItemsView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                            {
                                Name = "Ebay-StartSelling",
                                DisplayName = $"Start Selling Item On Ebay",
                                Description = string.Empty,
                                IsEnabled = true,
                                RequiresConfirmation = false,
                                EntityView = "Ebay-FormStartSelling",
                                UiHint = string.Empty,
                                Icon = "shelf_full"
                            });

                            sellableItemsView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
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

                            sellableItemsView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                            {
                                Name = "Ebay-StartSellingAll",
                                DisplayName = $"List All Items on Ebay for this Category ",
                                Description = string.Empty,
                                IsEnabled = true,
                                RequiresConfirmation = false,
                                EntityView = "Ebay-FormStartSellingAll",
                                UiHint = string.Empty,
                                Icon = "shelf_full"
                            });

                            sellableItemsView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                            {
                                Name = "Ebay-EndListingAllItems",
                                DisplayName = $"End All Listings on Ebay for this Category",
                                Description = string.Empty,
                                IsEnabled = true,
                                RequiresConfirmation = true,
                                EntityView = string.Empty,
                                UiHint = string.Empty,
                                Icon = "shelf_empty"
                            });
                        }

                        return entityView;
                    }

                }
            }

            return entityView;
        }
    }
}
