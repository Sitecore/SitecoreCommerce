// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionAddChildViewFromTemplateBlock.cs" company="Sitecore Corporation">
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
    /// Defines the do action add child view from template block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(ComposerConstants.Pipelines.Blocks.DoActionAddChildViewFromTemplateBlock)]
    public class DoActionAddChildViewFromTemplateBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionAddChildViewFromTemplateBlock"/> class.
        /// </summary>
        /// <param name="commander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionAddChildViewFromTemplateBlock(CommerceCommander commander)
        {
            this._commander = commander;
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
                || string.IsNullOrEmpty(entityView.Name)
                || !entityView.Name.Equals(context.GetPolicy<KnownComposerViewsPolicy>().AddChildViewFromTemplate, StringComparison.OrdinalIgnoreCase)
                || !entityView.Action.Equals(context.GetPolicy<KnownComposerActionsPolicy>().AddChildViewFromTemplate, StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            var entity = context.CommerceContext.GetObjects<CommerceEntity>().FirstOrDefault(p => p.Id.Equals(entityView.EntityId, StringComparison.OrdinalIgnoreCase));
            if (entity == null)
            {
                return entityView;
            }

            var templateName = entityView.Properties.FirstOrDefault(p => p.Name.Equals("Template", StringComparison.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(templateName?.Value))
            {
                var propertyDisplayName = templateName == null ? "Template" : templateName.DisplayName;
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().ValidationError,
                    "InvalidOrMissingPropertyValue",
                    new object[] { "Template" },
                    $"Invalid or missing value for property '{propertyDisplayName}'.");
                return entityView;
            }

            var template = await this._commander.GetEntity<ComposerTemplate>(context.CommerceContext, $"{CommerceEntity.IdPrefix<ComposerTemplate>()}{templateName.Value}");
            if (template == null)
            {
                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().ValidationError,
                    "InvalidOrMissingPropertyValue",
                    new object[] { "Template" },
                    $"Invalid or missing value for property '{templateName.DisplayName}'.");
                return entityView;
            }
            
            var childView = template.GetComponent<EntityViewComponent>().View;
            childView.EntityId = entity.Id;
            childView.DisplayRank = 0;
            childView.ItemId = $"Composer-{Guid.NewGuid():N}";

            entity.GetComponent<EntityViewComponent>().View.ChildViews.Add(childView);
            await this._commander.PersistEntity(context.CommerceContext, entity);

            return entityView;
        }
    }
}
