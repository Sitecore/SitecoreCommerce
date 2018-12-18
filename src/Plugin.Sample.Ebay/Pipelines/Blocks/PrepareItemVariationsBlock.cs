namespace Plugin.Sample.Ebay.Pipelines.Blocks
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using eBay.Service.Core.Soap;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("Ebay.PrepareItemVariationsBlock")]
    public class PrepareItemVariationsBlock : PipelineBlock<ItemType, ItemType, CommercePipelineExecutionContext>
    {
        public override async Task<ItemType> Run(ItemType item, CommercePipelineExecutionContext context)
        {
            Condition.Requires(item).IsNotNull($"{Name}: The argument can not be null");
            try
            {
                var foundEntity = context.CommerceContext.GetObjects<FoundEntity>().FirstOrDefault(p => p.Entity is SellableItem);
                if (foundEntity != null)
                {
                    var sellableItem = foundEntity.Entity as SellableItem;
                    if (sellableItem.HasComponent<ItemVariationsComponent>())
                    {
                        // This item has variations
                        item.Variations = new VariationsType { Variation = new VariationTypeCollection() };

                        var itemVariationsComponent = sellableItem.GetComponent<ItemVariationsComponent>();

                        var allColors = new StringCollection();

                        foreach (var variationComponent in itemVariationsComponent.ChildComponents.OfType<ItemVariationComponent>())
                        {
                            var newVariation = new VariationType { VariationTitle = variationComponent.DisplayName };

                            var listPricingPolicy = sellableItem.GetPolicy<ListPricingPolicy>();
                            var listPrice = listPricingPolicy.Prices.FirstOrDefault();

                            newVariation.StartPrice = new AmountType
                            {
                                currencyID = CurrencyCodeType.USD,
                                Value = System.Convert.ToDouble(listPrice.Amount)
                            };

                            newVariation.SKU = variationComponent.Id;
                            newVariation.Quantity = 10;
                            newVariation.VariationSpecifics = new NameValueListTypeCollection();
                            var displayPropertiesComponent = variationComponent.GetComponent<DisplayPropertiesComponent>();

                            if (string.IsNullOrEmpty(displayPropertiesComponent.Color))
                            {
                                displayPropertiesComponent.Color = "None";
                            }

                            newVariation.VariationSpecifics.Add(new NameValueListType() { Name = "Color", Value = new StringCollection() { displayPropertiesComponent.Color } });
                            
                            if (!allColors.Contains(displayPropertiesComponent.Color))
                            {
                                allColors.Add(displayPropertiesComponent.Color);
                            }

                            item.Variations.Variation.Add(newVariation);

                            item.Variations.VariationSpecificsSet = new NameValueListTypeCollection();
                            item.Variations.VariationSpecificsSet.Add(new NameValueListType() { Name = "Color", Value = allColors });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Ebay.PrepareItemVariationsBlock.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage("Error", "PrepareItemVariationsBlock.Run.Exception", new object[] { ex }, ex.Message);
            }

            return item;
        }
    }
}
