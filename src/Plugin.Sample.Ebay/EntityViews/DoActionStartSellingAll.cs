namespace Plugin.Sample.Ebay.EntityViews
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Ebay.Commands;
    using global::Plugin.Sample.Ebay.Components;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionStartSellingAll")]
    public class DoActionStartSellingAll : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionStartSellingAll(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("Ebay-StartSellingAll", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var immediateListing = entityView.Properties.First(p => p.Name == "ImmediateListing").Value ?? "";
                var isImmediateListing = System.Convert.ToBoolean(immediateListing);

                var foundEntity = context.CommerceContext.GetObjects<FoundEntity>().FirstOrDefault(p => p.EntityId == entityView.EntityId);
                if (foundEntity != null)
                {
                    var category = foundEntity.Entity as Category;

                    var listName = $"{CatalogConstants.Relationships.CategoryToSellableItem}-{category.Id.SimplifyEntityName()}";

                    var sellableItems = await this._commerceCommander.Command<ListCommander>()
                        .GetListItems<SellableItem>(context.CommerceContext, listName, 0,10);

                    foreach (var sellableItem in sellableItems)
                    {
                        if (isImmediateListing)
                        {
                            if (sellableItem.HasComponent<EbayItemComponent>())
                            {
                                var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();
                                if (ebayItemComponent.Status == "Ended")
                                {
                                    try
                                    {
                                        await this._commerceCommander.Command<EbayCommand>().RelistItem(context.CommerceContext, sellableItem);
                                    }
                                    catch (Exception ex)
                                    {
                                        context.Logger.LogError($"Ebay.DoActionStartSelling.Exception: Message={ex.Message}");
                                        await context.CommerceContext.AddMessage("Error", "DoActionStartSelling.Run.Exception", new object[] { ex }, ex.Message);
                                    }
                                }
                                else
                                {
                                    await this._commerceCommander.Command<EbayCommand>().AddItem(context.CommerceContext, sellableItem);
                                }
                            }
                            else
                            {
                                await this._commerceCommander.Command<EbayCommand>().AddItem(context.CommerceContext, sellableItem);
                            }
                        }
                        else
                        {
                            var ebayItemComponent = sellableItem.GetComponent<EbayItemComponent>();
                            ebayItemComponent.Status = "Pending";
                            sellableItem.GetComponent<TransientListMembershipsComponent>().Memberships.Add("Ebay_Pending");
                        }

                        await this._commerceCommander.PersistEntity(context.CommerceContext, sellableItem);
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionStartSellingAll.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "DoActionStartSelling.Run.Exception", new object[] { ex }, ex.Message);
            }

            return entityView;
        }
    }
}
