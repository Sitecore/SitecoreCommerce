// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetComposerDashboardViewBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Composer
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the get composer dashboard view block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(ComposerConstants.Pipelines.Blocks.GetComposerDashboardViewBlock)]
    public class GetComposerDashboardViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly ListCommander _commander;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetComposerDashboardViewBlock"/> class.
        /// </summary>
        /// <param name="commander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public GetComposerDashboardViewBlock(ListCommander commander)
        {
            this._commander = commander;
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="entityView">The entity view</param>
        /// <param name="context">The context</param>
        /// <returns>A <see cref="EntityView"/></returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            var views = context.GetPolicy<KnownComposerViewsPolicy>();
            if (string.IsNullOrEmpty(entityView.Name)
                || !entityView.Name.Equals(views.ComposerDashboard, StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();
            entityView.Icon = pluginPolicy.Icon;

            var templatesView = new EntityView
            {
                Name = views.ComposerTemplates,
                UiHint = "Table",
                Icon = pluginPolicy.Icon,
                ItemId = string.Empty,
                EntityId = string.Empty,
                DisplayRank = 0
            };
            entityView.ChildViews.Add(templatesView);

            var templates = await this._commander.GetListItems<ComposerTemplate>(context.CommerceContext, CommerceEntity.ListName<ComposerTemplate>(), 0, 10);
            foreach (var template in templates)
            {
                templatesView.ChildViews.Add(
                    new EntityView
                        {
                            ItemId = template.Id,
                            Icon = pluginPolicy.Icon,
                            Properties = new List<ViewProperty>
                                             {
                                                 new ViewProperty { Name = "ItemId", RawValue = template.Id, IsHidden = true },
                                                 new ViewProperty { Name = "Name", RawValue = template.Name, UiType = "EntityLink" },
                                                 new ViewProperty { Name = "DisplayName", RawValue = template.DisplayName }
                                             }
                        });
            }

            return entityView;
        }
    }
}
