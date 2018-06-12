// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionAddSelectionOptionPropertyConstrainBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Composer
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Views;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the do action add selection option property constrain block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(ComposerConstants.Pipelines.Blocks.DoActionAddSelectionOptionPropertyConstrainBlock)]
    public class DoActionAddSelectionOptionPropertyConstrainBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionAddSelectionOptionPropertyConstrainBlock"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionAddSelectionOptionPropertyConstrainBlock(CommerceCommander commerceCommander)
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
            if (string.IsNullOrEmpty(entityView?.Action)
                || string.IsNullOrEmpty(entityView.EntityId)
                || string.IsNullOrEmpty(entityView.ItemId)
                || string.IsNullOrEmpty(entityView.Name)
                || !entityView.Name.Equals(context.GetPolicy<KnownComposerViewsPolicy>().AddSelectionOptionPropertyConstrain, StringComparison.OrdinalIgnoreCase)
                || !entityView.Action.Equals(context.GetPolicy<KnownComposerActionsPolicy>().AddSelectionOptionPropertyConstraint, StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            var entity = context.CommerceContext.GetObjects<CommerceEntity>().FirstOrDefault(p => p.Id.Equals(entityView.EntityId, StringComparison.OrdinalIgnoreCase));
            if (entity == null)
            {
                return entityView;
            }

            var viewComponent = entity.GetComponent<EntityViewComponent>();
            var targetChildView = viewComponent.View.ChildViews.OfType<EntityView>()
                .FirstOrDefault(p => p.ItemId.Equals(entityView.ItemId, StringComparison.OrdinalIgnoreCase));
            if (targetChildView == null)
            {
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().ValidationError,
                    "InvalidOrMissingPropertyValue",
                    new object[] { "ItemId" },
                    "Invalid or missing value for property 'ItemId'.");
                return entityView;
            }

            var propertyName = entityView.Properties.FirstOrDefault(p => p.Name.Equals("Property", StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(propertyName?.Value))
            {
                var propertyDisplayName = propertyName == null ? "Property" : propertyName.DisplayName;
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().ValidationError,
                    "InvalidOrMissingPropertyValue",
                    new object[] { "Property" },
                    $"Invalid or missing value for property '{propertyDisplayName}'.");
                return entityView;
            }

            var property = targetChildView.Properties.FirstOrDefault(p => p.Name.Equals(propertyName.Value, StringComparison.OrdinalIgnoreCase));
            if (property == null)
            {
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().ValidationError,
                    "InvalidOrMissingPropertyValue",
                    new object[] { "Property" },
                    $"Invalid or missing value for property '{propertyName.DisplayName}'.");
                return entityView;
            }

            if (!property.OriginalType.Equals("System.String", StringComparison.OrdinalIgnoreCase))
            {
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().ValidationError,
                    "SelectionOptionConstrainOnlyForNumericProperties",
                    null,
                    "Min-Max constrain can only be added to numeric properties.");
                return entityView;
            }
            
            var name = entityView.Properties.FirstOrDefault(p => p.Name.Equals("OptionName", StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(name?.Value))
            {
                var propertyDisplayName = name == null ? "OptionName" : name.DisplayName;
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().ValidationError,
                    "InvalidOrMissingPropertyValue",
                    new object[] { "OptionName" },
                    $"Invalid or missing value for property '{propertyDisplayName}'.");
                return entityView;
            }
            
            var value = entityView.Properties.FirstOrDefault(p => p.Name.Equals("OptionValue", StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(value?.Value))
            {
                var propertyDisplayName = value == null ? "OptionValue" : value.DisplayName;
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().ValidationError,
                    "InvalidOrMissingPropertyValue",
                    new object[] { "OptionValue" },
                    $"Invalid or missing value for property '{propertyDisplayName}'.");
                return entityView;
            }

            var selectionPolicy = property.GetPolicy<AvailableSelectionsPolicy>();
            if (selectionPolicy.List.Any(o => o.Name.Equals(value) || o.DisplayName.Equals(name)))
            {
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().ValidationError,
                    "SelectionOptionAlreadyExists",
                    new object[] { value, name },
                    $"A selection option with value '{value}' or name '{name}' already exists.");
                return entityView;
            }

            selectionPolicy.List.Add(new Selection { DisplayName = name.Value, Name = value.Value });

            await this._commerceCommander.PersistEntity(context.CommerceContext, entity);

            return entityView;
        }
    }
}
