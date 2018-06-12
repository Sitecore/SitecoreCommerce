// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopulateComposerEntityViewActionsBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Composer
{
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the populate composer view actions block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(ComposerConstants.Pipelines.Blocks.PopulateComposerEntityViewActionsBlock)]
    public class PopulateComposerEntityViewActionsBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="PopulateComposerEntityViewActionsBlock"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public PopulateComposerEntityViewActionsBlock(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The run.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            var request = context.CommerceContext.GetObjects<EntityViewArgument>().FirstOrDefault();
            if (request?.Entity == null
                || !string.IsNullOrEmpty(request.ForAction)
                || !string.IsNullOrEmpty(entityView.Action)
                || string.IsNullOrEmpty(entityView.EntityId)
                || !entityView.EntityId.Contains("Entity-"))
            {
                return entityView;
            }
            
            var actions = context.GetPolicy<KnownComposerActionsPolicy>();
            var views = context.GetPolicy<KnownComposerViewsPolicy>();
            var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();

            var template = await this._commerceCommander
                               .GetEntity<ComposerTemplate>(context.CommerceContext, $"{CommerceEntity.IdPrefix<ComposerTemplate>()}BaseNaming");
            actionsPolicy.Actions.Add(new EntityActionView
            {
                Name = actions.AddChildView,
                IsEnabled = template != null,
                RequiresConfirmation = false,
                EntityView = views.AddChildView,
                Icon = "window"
            });

            var templates = await this._commerceCommander.Command<ListCommander>()
                                .GetListItemIds<ComposerTemplate>(context.CommerceContext, CommerceEntity.ListName<ComposerTemplate>(), 0, 1);
            actionsPolicy.Actions.Add(new EntityActionView
            {
                Name = actions.AddChildViewFromTemplate,
                IsEnabled = templates != null && templates.Any(),
                RequiresConfirmation = false,
                EntityView = views.AddChildViewFromTemplate,
                Icon = "window_star"
            });

            return entityView;
        }
    }
}
