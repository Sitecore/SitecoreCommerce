
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Sitecore.Commerce.Plugin.Ebay
{
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System;
    using System.Linq;


    /// <summary>
    /// Defines a block which populates an EntityView for a Sample Page in the Sitecore Commerce Focused Commerce Experience.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("CategoryEbayExtensions")]
    public class CategoryEbayExtensions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {

        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryEbayExtensions"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public CategoryEbayExtensions(CommerceCommander commerceCommander)
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

            try
            {

                if (entityView.EntityId.Contains("Entity-Category-"))
                {
                    var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(context.CommerceContext, "Entity-EbayConfigEntity-Global", true);

                    if (ebayConfig.HasComponent<EbayBusinessUserComponent>())
                    {
                        var ebayConfigComponent = ebayConfig.GetComponent<EbayBusinessUserComponent>();

                        var sellableItemsView = entityView.ChildViews.FirstOrDefault(p => p.Name == "SellableItems") as EntityView;
                        if (sellableItemsView != null)
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
                                                Name = "",
                                                IsHidden = false,
                                                IsReadOnly = true,
                                                UiType = "",
                                                RawValue = ""
                                            });
                                        }
                                    }
                                    else
                                    {
                                        sellableItemViewItemAsEntityView.Properties.Insert(0,
                                            new ViewProperty
                                            {
                                                Name = "",
                                                IsHidden = false,
                                                IsReadOnly = true,
                                                UiType = "",
                                                RawValue = ""
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
                await context.CommerceContext.AddMessage("Error", "CategoryEbayExtensions.Run.Exception", new Object[] { ex }, ex.Message);
            }
            return entityView;

        }
    }
}
