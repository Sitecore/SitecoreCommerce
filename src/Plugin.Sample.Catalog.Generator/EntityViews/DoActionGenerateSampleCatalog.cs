namespace Plugin.Sample.Catalog.Generator.EntityViews
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Bogus;

    using Microsoft.Extensions.Logging;

    using Plugin.Sample.Catalog.Generator.Commands;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("DoActionGenerateSampleCatalog")]
    public class DoActionGenerateSampleCatalog : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionGenerateSampleCatalog(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("Catalog-GenerateSampleCatalog", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var categoriesCacheList = new System.Collections.Generic.List<Category>();
                var name = entityView.Properties.First(p => p.Name == "Name").Value ?? "";
                var displayName = entityView.Properties.First(p => p.Name == "DisplayName").Value ?? "";
                var categories = entityView.Properties.First(p => p.Name == "Categories").Value ?? "";
                var products = entityView.Properties.First(p => p.Name == "Products").Value ?? "";

                Randomizer.Seed = new Random();

                var createdCatalog = await this._commerceCommander.Command<CreateCatalogCommand>().Process(context.CommerceContext, name, displayName);

                createdCatalog = await this._commerceCommander.Command<GetCatalogCommand>().Process(context.CommerceContext, name);

                var createSellableItemCommand = this._commerceCommander.Command<CreateSellableItemCommand>();
                var createCategoryCommand = this._commerceCommander.Command<CreateCategoryCommand>();

                var categoriesRequested = System.Convert.ToInt32(categories);
                for (int i = 1; i <= categoriesRequested; i++)
                {
                    var fakeCategory = this._commerceCommander.Command<GetSampleCategoryCommand>().Process(context.CommerceContext, null);
                    var catDupeCount = 0;
                    while (catDupeCount < 10 && categoriesCacheList.Any(p=>p.Name == fakeCategory.Name))
                    {
                        catDupeCount++;
                        fakeCategory = this._commerceCommander.Command<GetSampleCategoryCommand>().Process(context.CommerceContext, null);
                    }
                    var createdCategory = await createCategoryCommand.Process(context.CommerceContext, createdCatalog.Id, fakeCategory.Name, fakeCategory.DisplayName, "description");

                    categoriesCacheList.Add(createdCategory);

                    if (categoriesCacheList.Count > 1 && new Random().Next(4) < 2)
                    {
                        var parentCategory = categoriesCacheList[new Random().Next(categoriesCacheList.Count - 1)];
                        await this._commerceCommander.Command<AssociateCategoryToParentCommand>().Process(context.CommerceContext, createdCatalog.Id, parentCategory.Id, createdCategory.Id);
                    }
                    else
                    {
                        await this._commerceCommander.Command<AssociateCategoryToParentCommand>().Process(context.CommerceContext, createdCatalog.Id, createdCatalog.Id, createdCategory.Id);
                    }
                }
                
                var productsRequested = System.Convert.ToInt32(products);
                for (int i = 1; i <= productsRequested; i++)
                {
                    var fakeSellableItem = this._commerceCommander.Command<GetSampleProductCommand>().Process(context.CommerceContext, null);

                    var createItemResult = await createSellableItemCommand.Process(context.CommerceContext, fakeSellableItem.Id, fakeSellableItem.Name, fakeSellableItem.DisplayName, "description", "");

                    var categoryToAssociateWith = categoriesCacheList[new Random().Next(categoriesCacheList.Count - 1)];

                    await this._commerceCommander.Command<AssociateSellableItemToParentCommand>().Process(context.CommerceContext, createdCatalog.Id, categoryToAssociateWith.Id, createItemResult.Id);
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionGeneratorSampleCatalo.Exception: Message={ex.Message}");
                await context.CommerceContext.AddMessage(
                               context.CommerceContext.GetPolicy<KnownResultCodes>().Error,
                               "EntityNotFound",
                               new object[] { ex },
                               $"Catalog was not Generated ({ex.Message})");
            }

            return entityView;
        }
    }
}
