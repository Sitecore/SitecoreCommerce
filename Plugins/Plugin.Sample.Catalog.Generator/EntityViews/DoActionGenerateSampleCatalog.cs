// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionGenerateSampleCatalog.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.CatalogGenerator
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Commerce.EntityViews;
    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Plugin.Catalog;
    using Bogus;

    /// <summary>
    /// Defines the do action edit line item block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("DoActionGenerateSampleCatalog")]
    public class DoActionGenerateSampleCatalog : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionGenerateSampleCatalog"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionGenerateSampleCatalog(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="entityView">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="EntityView"/>.
        /// </returns>
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

                //var entityViewComponent = storedEntity.GetComponent<EntityViewComponent>();
                var name = entityView.Properties.First(p => p.Name == "Name").Value ?? "";
                var displayName = entityView.Properties.First(p => p.Name == "DisplayName").Value ?? "";
                var categories = entityView.Properties.First(p => p.Name == "Categories").Value ?? "";
                var products = entityView.Properties.First(p => p.Name == "Products").Value ?? "";

                Randomizer.Seed = new Random();

                var createdCatalog = await this._commerceCommander.Command<CreateCatalogCommand>().Process(context.CommerceContext, name, displayName, "", "");

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
                        //duplicate category name
                        catDupeCount++;
                        fakeCategory = this._commerceCommander.Command<GetSampleCategoryCommand>().Process(context.CommerceContext, null);
                    }
                    var createdCategory = await createCategoryCommand.Process(context.CommerceContext, createdCatalog.Id, fakeCategory.Name, fakeCategory.DisplayName, "description", true);

                    categoriesCacheList.Add(createdCategory);

                    if (categoriesCacheList.Count > 1 && new Random().Next(4) < 2)
                    {
                        //should be a child category of another category
                        var parentCategory = categoriesCacheList[new Random().Next(categoriesCacheList.Count - 1)];
                        var associateResult = await this._commerceCommander.Command<AssociateCategoryToParentCommand>().Process(context.CommerceContext, createdCatalog.Id, parentCategory.Id, createdCategory.Id);

                    }
                    else
                    {
                        //should be a root category
                        var associateResult = await this._commerceCommander.Command<AssociateCategoryToParentCommand>().Process(context.CommerceContext, createdCatalog.Id, createdCatalog.Id, createdCategory.Id);
                    }
                    
                }


                var productsRequested = System.Convert.ToInt32(products);
                for (int i = 1; i <= productsRequested; i++)
                {
                    var fakeSellableItem = this._commerceCommander.Command<GetSampleProductCommand>().Process(context.CommerceContext, null);

                    var createItemResult = await createSellableItemCommand.Process(context.CommerceContext, fakeSellableItem.Id, fakeSellableItem.Name, fakeSellableItem.DisplayName, "description", "");

                    var categoryToAssociateWith = categoriesCacheList[new Random().Next(categoriesCacheList.Count - 1)];

                    var associateResults = await this._commerceCommander.Command<AssociateSellableItemToParentCommand>().Process(context.CommerceContext, createdCatalog.Id, categoryToAssociateWith.Id, createItemResult.Id);

                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionGeneratorSampleCatalo.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
