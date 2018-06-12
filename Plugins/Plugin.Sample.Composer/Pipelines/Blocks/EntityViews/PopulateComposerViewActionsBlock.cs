// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PopulateComposerViewActionsBlock.cs" company="Sitecore Corporation">
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
    /// Defines the populate composer view actions block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(ComposerConstants.Pipelines.Blocks.PopulateComposerViewActionsBlock)]
    public class PopulateComposerViewActionsBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        /// <summary>The run.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (!string.IsNullOrEmpty(entityView.Action))  //|| string.IsNullOrEmpty(entityView.ItemId)
            {
                return Task.FromResult(entityView);
            }

            if (!entityView.ItemId.Contains("Composer-") && !entityView.EntityId.Contains("Entity-ComposerTemplate-"))
            {
                return Task.FromResult(entityView);
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();
            var actions = context.GetPolicy<KnownComposerActionsPolicy>();
            var views = context.GetPolicy<KnownComposerViewsPolicy>();

            var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();
            actionsPolicy.Actions.Add(new EntityActionView
            {
                Name = actions.EditView,
                IsEnabled = true,
                RequiresConfirmation = false,
                EntityView = views.EditView,
                Icon = pluginPolicy.Icon
            });

            actionsPolicy.Actions.Add(new EntityActionView
            {
                Name = actions.RemoveView,
                IsEnabled = true,
                RequiresConfirmation = true,
                EntityView = string.Empty,
                Icon = pluginPolicy.Icon
            });
            
            actionsPolicy.Actions.Add(new EntityActionView
            {
                Name = actions.MakeTemplate,
                IsEnabled = true,
                RequiresConfirmation = false,
                EntityView = views.MakeTemplate,
                Icon = pluginPolicy.Icon
            });

            actionsPolicy.Actions.Add(new EntityActionView
            {
                Name = actions.AddProperty,
                IsEnabled = true,
                RequiresConfirmation = false,
                EntityView = views.AddProperty,
                Icon = pluginPolicy.Icon
            });

            actionsPolicy.Actions.Add(new EntityActionView
            {
                Name = actions.RemoveProperty,
                IsEnabled = true,
                RequiresConfirmation = false,
                EntityView = views.RemoveProperty,
                Icon = pluginPolicy.Icon
            });

            var hasNumericProperties = entityView.Properties.Any(
                    p => p.OriginalType.Equals("System.Int32", StringComparison.OrdinalIgnoreCase)
                || p.OriginalType.Equals("System.Decimal", StringComparison.OrdinalIgnoreCase));
            actionsPolicy.Actions.Add(new EntityActionView
            {
                Name = actions.AddMinMaxPropertyConstraint,
                IsEnabled = hasNumericProperties,
                RequiresConfirmation = false,
                EntityView = views.AddMinMaxPropertyConstrain,
                Icon = pluginPolicy.Icon
            });

            var hasStringProperties = entityView.Properties.Any(
                p => p.OriginalType.Equals("System.String", StringComparison.OrdinalIgnoreCase));
            actionsPolicy.Actions.Add(new EntityActionView
            {
                Name = actions.AddSelectionOptionPropertyConstraint,
                IsEnabled = hasStringProperties,
                RequiresConfirmation = false,
                EntityView = views.AddSelectionOptionPropertyConstrain,
                Icon = pluginPolicy.Icon
            });

            return Task.FromResult(entityView);
        }
    }
}
