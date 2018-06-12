// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnsureActionsCategory.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.Commerce.Plugin.Ebay
{
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.BusinessUsers;

    /// <summary>
    /// Defines a block which validates, inserts and updates Actions for this Plugin.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("EnsureActionsCategory")]
    public class EnsureActionsCategory : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsureActionsCategory"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EnsureActionsCategory(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            if (entityView.EntityId.Contains("Entity-Category-"))
            {
                var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(context.CommerceContext, "Entity-EbayConfigEntity-Global", true);

                if (ebayConfig.HasComponent<EbayBusinessUserComponent>())
                {
                    var ebayConfigComponent = ebayConfig.GetComponent<EbayBusinessUserComponent>();
                    if (!string.IsNullOrEmpty(ebayConfigComponent.EbayToken))
                    {

                        var sellableItemsView = entityView.ChildViews.FirstOrDefault(p => p.Name == "SellableItems") as EntityView;
                        if (sellableItemsView != null)
                        {
                            sellableItemsView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                            {
                                Name = "Ebay-StartSelling",
                                DisplayName = $"Start Selling Item On Ebay",
                                Description = "",
                                IsEnabled = true,
                                RequiresConfirmation = false,
                                EntityView = "Ebay-FormStartSelling",
                                UiHint = "",
                                Icon = "shelf_full"
                            });

                            sellableItemsView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                            {
                                Name = "Ebay-EndItem",
                                DisplayName = $"End Item Listing on Ebay",
                                Description = "",
                                IsEnabled = true,
                                RequiresConfirmation = false,
                                EntityView = "Ebay-FormEndItem",
                                UiHint = "",
                                Icon = "shelf_empty"
                            });

                            sellableItemsView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                            {
                                Name = "Ebay-StartSellingAll",
                                DisplayName = $"List All Items on Ebay for this Category ",
                                Description = "",
                                IsEnabled = true,
                                RequiresConfirmation = false,
                                EntityView = "Ebay-FormStartSellingAll",
                                UiHint = "",
                                Icon = "shelf_full"
                            });

                            sellableItemsView.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                            {
                                Name = "Ebay-EndListingAllItems",
                                DisplayName = $"End All Listings on Ebay for this Category",
                                Description = "",
                                IsEnabled = true,
                                RequiresConfirmation = true,
                                EntityView = "",
                                UiHint = "",
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
