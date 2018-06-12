// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetComposerAddChildViewFromTemplateViewBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Composer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines get composer add child view from template view block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(ComposerConstants.Pipelines.Blocks.GetComposerAddChildViewFromTemplateViewBlock)]
    public class GetComposerAddChildViewFromTemplateViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly ListCommander _commander;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetComposerAddChildViewFromTemplateViewBlock"/> class.
        /// </summary>
        /// <param name="commander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public GetComposerAddChildViewFromTemplateViewBlock(ListCommander commander)
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

            if (string.IsNullOrEmpty(entityView.Name)
                || string.IsNullOrEmpty(entityView.Action)
                || !entityView.Name.Equals(context.GetPolicy<KnownComposerViewsPolicy>().AddChildViewFromTemplate, StringComparison.OrdinalIgnoreCase)
                || !entityView.Action.Equals(context.GetPolicy<KnownComposerActionsPolicy>().AddChildViewFromTemplate, StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            var templates = (await this._commander.GetListItems<ComposerTemplate>(context.CommerceContext, CommerceEntity.ListName<ComposerTemplate>(), 0, int.MaxValue)).ToList();
            var templateProperty = new ViewProperty
            {
                Name = "Template",
                IsHidden = false,
                IsRequired = true,
                RawValue = string.Empty,
                Policies = new List<Policy>
                               {
                                   new AvailableSelectionsPolicy
                                       {
                                           List = templates
                                               .Select(t => new Selection { DisplayName = t.DisplayName, Name = t.Name }).ToList()
                                       }
                               }
            };
            entityView.Properties.Add(templateProperty);

            return entityView;
        }
    }
}
