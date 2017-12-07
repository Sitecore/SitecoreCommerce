// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormGenerateSampleCatalog.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.CatalogGenerator
{
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using System;

    /// <summary>
    /// Defines a block which populates an EntityView for a list of Coupons with the View named Public Coupons.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("FormGenerateSampleCatalog")]
    public class FormGenerateSampleCatalog : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormGenerateSampleCatalog"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public FormGenerateSampleCatalog(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "Catalog-GenerateSampleCatalog")
            {
                return Task.FromResult(entityView);
            }

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Name",
                    IsHidden = false,
                    //IsReadOnly = true,
                    IsRequired = false,
                    RawValue = ""
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "DisplayName",
                    IsHidden = false,
                    //IsReadOnly = true,
                    IsRequired = false,
                    RawValue = ""
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Categories",
                    IsHidden = false,
                    //IsReadOnly = true,
                    IsRequired = false,
                    RawValue = 100
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Products",
                    IsHidden = false,
                    //IsReadOnly = true,
                    IsRequired = false,
                    RawValue = 100
                });

            return Task.FromResult(entityView);
        }
    }

}
