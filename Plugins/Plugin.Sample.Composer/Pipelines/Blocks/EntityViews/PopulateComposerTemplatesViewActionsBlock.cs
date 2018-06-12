// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopulateComposerTemplatesViewActionsBlock.cs" company="Sitecore Corporation">
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
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the populate composer templates view actions block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(ComposerConstants.Pipelines.Blocks.PopulateComposerTemplatesViewActionsBlock)]
    public class PopulateComposerTemplatesViewActionsBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly ListCommander _commander;

        /// <summary>
        /// Initializes a new instance of the <see cref="PopulateComposerTemplatesViewActionsBlock"/> class.
        /// </summary>
        /// <param name="commander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public PopulateComposerTemplatesViewActionsBlock(ListCommander commander)
        {
            this._commander = commander;
        }

        /// <summary>The run.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (string.IsNullOrEmpty(entityView?.Name)
                || !string.IsNullOrEmpty(entityView.Action)
                || !entityView.Name.Equals(context.GetPolicy<KnownComposerViewsPolicy>().ComposerTemplates, StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            var actions = context.GetPolicy<KnownComposerActionsPolicy>();

            var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();
            actionsPolicy.Actions.Add(new EntityActionView
            {
                Name = actions.EnsureDefaultTemplates,
                IsEnabled = true,
                RequiresConfirmation = true,
                EntityView = string.Empty,
                Icon = "add"
            });

            var templates = await this._commander.GetListItemIds<ComposerTemplate>(context.CommerceContext, CommerceEntity.ListName<ComposerTemplate>(), 0, 1);
            actionsPolicy.Actions.Add(new EntityActionView
            {
                Name = actions.ClearTemplates,
                IsEnabled = templates != null && templates.Any(),
                RequiresConfirmation = true,
                EntityView = string.Empty,
                Icon = "garbage"
            });

            actionsPolicy.Actions.Add(new EntityActionView
            {
                Name = actions.RemoveTemplate,
                IsEnabled = templates != null && templates.Any(),
                RequiresConfirmation = true,
                EntityView = string.Empty,
                Icon = "delete"
            });
            
            return entityView;
        }
    }
}
