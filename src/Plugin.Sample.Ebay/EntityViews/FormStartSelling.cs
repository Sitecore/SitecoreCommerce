
namespace Plugin.Sample.Ebay.EntityViews
{
    using System.Linq;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Ebay.Commands;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Inventory;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("FormStartSelling")]
    public class FormStartSelling : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public FormStartSelling(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "Ebay-FormStartSelling")
            {
                return entityView;
            }

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            var inventoryAvailable = 0;
            var sellableItem = entityViewArgument.Entity as SellableItem;

            if (sellableItem == null)
            {
                sellableItem = await this._commerceCommander.GetEntity<SellableItem>(context.CommerceContext, entityView.ItemId);
            }

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = string.Empty,
                    IsHidden = false,
                    IsReadOnly = true,
                    OriginalType = "Html",
                    UiType = "Html",
                    RawValue = "<img alt='This is the alternate' height=50 width=100 src='https://www.paypalobjects.com/webstatic/en_AR/mktg/merchant/pages/sell-on-ebay/image-ebay.png' style=''/>"
                });

            if (sellableItem.HasComponent<ItemVariationsComponent>())
            {
                // has variations
                foreach(var itemVariationComponent in sellableItem.GetComponent<ItemVariationsComponent>().GetComponents<ItemVariationComponent>())
                {
                    if (itemVariationComponent.HasComponent<InventoryComponent>())
                    {
                        var inventoryComponent = itemVariationComponent.GetComponent<InventoryComponent>();
                        foreach(var inventoryAssociationTarget in inventoryComponent.InventoryAssociations)
                        {
                            var inventoryItem = await this._commerceCommander.GetEntity<InventoryInformation>(context.CommerceContext, inventoryAssociationTarget.InventoryInformation.EntityTarget);

                            if (inventoryAvailable < inventoryItem.Quantity)
                            {
                                inventoryAvailable = inventoryItem.Quantity;
                            }
                            
                            entityView.Properties.Add(
                                new ViewProperty
                                {
                                    Name = $"IncludeVariant-{itemVariationComponent.Id}",
                                    DisplayName = $"Include Variant-{itemVariationComponent.Name} ({inventoryItem.Quantity})",
                                    IsHidden = false,
                                    IsRequired = true,
                                    RawValue = true
                                });
                        }
                    }
                }
            }

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Quantity",
                    DisplayName = "Quantity to Sell",
                    IsHidden = false,
                    IsRequired = true,
                    RawValue = inventoryAvailable
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "ListingDuration",
                    DisplayName = "Listing Duration (Days)",
                    IsHidden = false,
                    IsRequired = true,
                    RawValue = 10
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Subtitle",
                    IsHidden = false,
                    IsRequired = true,
                    RawValue = "Item Default Subtitle"
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "ImmediateListing",
                    DisplayName = "Publish Immediately to Ebay",
                    IsHidden = false,
                    IsRequired = true,
                    RawValue = true
                });

            await this._commerceCommander.Command<EbayCommand>().GetSuggestedCategories(context.CommerceContext, sellableItem.Tags.First().Name);
            
            return entityView;
        }
    }
}
