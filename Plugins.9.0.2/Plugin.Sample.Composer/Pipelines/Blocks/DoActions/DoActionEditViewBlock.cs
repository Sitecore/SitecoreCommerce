// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionEditViewBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Composer
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Views;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the do action edit view block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(ComposerConstants.Pipelines.Blocks.DoActionEditViewBlock)]
    public class DoActionEditViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionEditViewBlock"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionEditViewBlock(CommerceCommander commerceCommander)
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
                || !entityView.Name.Equals(context.GetPolicy<KnownComposerViewsPolicy>().EditView, StringComparison.OrdinalIgnoreCase)
                || !entityView.Action.Equals(context.GetPolicy<KnownComposerActionsPolicy>().EditView, StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            var entity = context.CommerceContext.GetObjects<CommerceEntity>().FirstOrDefault(p => p.Id.Equals(entityView.EntityId, StringComparison.OrdinalIgnoreCase));
            if (entity == null)
            {
                return entityView;
            }

            var viewComponent = entity.GetComponent<EntityViewComponent>();
            var selectedView = viewComponent.View.ChildViews.OfType<EntityView>().FirstOrDefault(p => p.ItemId.Equals(entityView.ItemId, StringComparison.OrdinalIgnoreCase));
            if (selectedView == null)
            {
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().ValidationError,
                    "InvalidOrMissingPropertyValue",
                    new object[] { "ItemId" },
                    "Invalid or missing value for property 'ItemId'.");
                return entityView;
            }

            foreach (var viewProperty in entityView.Properties.Where(p => !p.Name.Equals("Version", StringComparison.OrdinalIgnoreCase)))
            {
                var property = selectedView.Properties.FirstOrDefault(p => p.Name.Equals(viewProperty.Name, StringComparison.OrdinalIgnoreCase));
                if (property == null)
                {
                    continue;
                }

                property.Value = viewProperty.Value;
                property.RawValue = null;
            }

            await this._commerceCommander.PersistEntity(context.CommerceContext, entity);

            return entityView;
        }
    }
}
