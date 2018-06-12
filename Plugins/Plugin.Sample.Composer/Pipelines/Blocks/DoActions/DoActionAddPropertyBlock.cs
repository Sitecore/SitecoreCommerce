// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionAddPropertyBlock.cs" company="Sitecore Corporation">
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
    /// Defines the do action add property block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(ComposerConstants.Pipelines.Blocks.DoActionAddPropertyBlock)]
    public class DoActionAddPropertyBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionAddPropertyBlock"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionAddPropertyBlock(CommerceCommander commerceCommander)
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
                || !entityView.Name.Equals(context.GetPolicy<KnownComposerViewsPolicy>().AddProperty, StringComparison.OrdinalIgnoreCase)
                || !entityView.Action.Equals(context.GetPolicy<KnownComposerActionsPolicy>().AddProperty, StringComparison.OrdinalIgnoreCase))
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

            var name = entityView.Properties.FirstOrDefault(p => p.Name.Equals("Name", StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(name?.Value))
            {
                var propertyDisplayName = name == null ? "Name" : name.DisplayName;
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().ValidationError,
                    "InvalidOrMissingPropertyValue",
                    new object[] { "Name" },
                    $"Invalid or missing value for property '{propertyDisplayName}'.");
                return entityView;
            }

            var propertyType = entityView.Properties.FirstOrDefault(p => p.Name.Equals("PropertyType", StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(propertyType?.Value))
            {
                var propertyDisplayName = propertyType == null ? "PropertyType" : propertyType.DisplayName;
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().ValidationError,
                    "InvalidOrMissingPropertyValue",
                    new object[] { "PropertyType" },
                    $"Invalid or missing value for property '{propertyDisplayName}'.");
                return entityView;
            }

            var displayName = entityView.Properties.FirstOrDefault(p => p.Name.Equals("DisplayName", StringComparison.OrdinalIgnoreCase))?.Value ?? string.Empty;

            var newProperty = new ViewProperty { Name = name.Value, DisplayName = displayName, OriginalType = propertyType.Value };

            switch (propertyType.Value)
            {
                case "System.Decimal":
                    newProperty.RawValue = 0M;
                    break;
                case "System.Int32":
                    newProperty.RawValue = 0;
                    break;
                case "System.Boolean":
                    newProperty.RawValue = false;
                    break;
                case "System.DateTimeOffset":
                    newProperty.RawValue = DateTimeOffset.UtcNow;
                    break;
                default:
                    newProperty.RawValue = string.Empty;
                    break;
            }

            targetChildView.Properties.Add(newProperty);
            await this._commerceCommander.PersistEntity(context.CommerceContext, entity);

            return entityView;
        }
    }
}
