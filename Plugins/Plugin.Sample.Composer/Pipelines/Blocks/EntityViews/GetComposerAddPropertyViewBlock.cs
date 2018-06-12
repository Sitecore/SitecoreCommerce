// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetComposerAddPropertyViewBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Composer
{
    using System;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines get composer add property view block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(ComposerConstants.Pipelines.Blocks.GetComposerAddPropertyViewBlock)]
    public class GetComposerAddPropertyViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        /// <summary>The run.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");
            
            if (string.IsNullOrEmpty(entityView.Name)
                || string.IsNullOrEmpty(entityView.Action)
                || !entityView.Name.Equals(context.GetPolicy<KnownComposerViewsPolicy>().AddProperty, StringComparison.OrdinalIgnoreCase)
                || !entityView.Action.Equals(context.GetPolicy<KnownComposerActionsPolicy>().AddProperty, StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(entityView);
            }
            
            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Name",
                    IsHidden = false,
                    IsRequired = true,
                    RawValue = string.Empty
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "DisplayName",
                    IsHidden = false,
                    IsRequired = false,
                    RawValue = string.Empty
                });

            var propertyType = new ViewProperty
            {
                Name = "PropertyType",
                IsHidden = false,
                IsRequired = true,
                RawValue = string.Empty
            };
            entityView.Properties.Add(propertyType);

            var availableSelections = propertyType.GetPolicy<AvailableSelectionsPolicy>();
            availableSelections.List.Add(new Selection { Name = "System.String", DisplayName = "String", IsDefault = true });
            availableSelections.List.Add(new Selection { Name = "System.DateTimeOffset", DisplayName = "DateTime", IsDefault = false });
            availableSelections.List.Add(new Selection { Name = "System.Decimal", DisplayName = "Decimal", IsDefault = false });
            availableSelections.List.Add(new Selection { Name = "System.Int32", DisplayName = "Int32", IsDefault = false });
            availableSelections.List.Add(new Selection { Name = "System.Boolean", DisplayName = "Boolean", IsDefault = false });

            return Task.FromResult(entityView);
        }
    }
}
