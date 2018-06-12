// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetComposerRemovePropertyViewBlock.cs" company="Sitecore Corporation">
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
    using Sitecore.Commerce.Plugin.Views;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines get composer remove property view block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(ComposerConstants.Pipelines.Blocks.GetComposerRemovePropertyViewBlock)]
    public class GetComposerRemovePropertyViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        /// <summary>The run.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            var request = context.CommerceContext.GetObjects<EntityViewArgument>().FirstOrDefault();
            if (string.IsNullOrEmpty(entityView.Name)
                || string.IsNullOrEmpty(entityView.Action)
                || !entityView.Name.Equals(context.GetPolicy<KnownComposerViewsPolicy>().RemoveProperty, StringComparison.OrdinalIgnoreCase)
                || !entityView.Action.Equals(context.GetPolicy<KnownComposerActionsPolicy>().RemoveProperty, StringComparison.OrdinalIgnoreCase)
                || string.IsNullOrEmpty(entityView.ItemId)
                || request?.Entity == null
                || !request.Entity.HasComponent<EntityViewComponent>())
            {
                return Task.FromResult(entityView);
            }

            var viewComponent = request.Entity.GetComponent<EntityViewComponent>();
            var selectedView = viewComponent.View.ChildViews.OfType<EntityView>().FirstOrDefault(p => p.ItemId.Equals(entityView.ItemId, StringComparison.OrdinalIgnoreCase));
            if (selectedView == null)
            {
                return Task.FromResult(entityView);
            }

            var property = new ViewProperty
                               {
                                   Name = "Property",
                                   RawValue = string.Empty,
                                   IsRequired = true,
                                   IsReadOnly = false,
                                   IsHidden = false,
                                   Policies = new List<Policy>
                                                  {
                                                      new AvailableSelectionsPolicy
                                                          {
                                                              List = selectedView.Properties.Select(p => new Selection { DisplayName = p.DisplayName, Name = p.Name }).ToList()
                                                          }
                                                  }
            };
            entityView.Properties.Add(property);

            return Task.FromResult(entityView);
        }
    }
}
