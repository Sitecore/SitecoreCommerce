// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetEntityComposerViewsBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Composer
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Views;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the get entity composer views block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(ComposerConstants.Pipelines.Blocks.GetEntityComposerViewsBlock)]
    public class GetEntityComposerViewsBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        /// <summary>The run.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            var request = context.CommerceContext.GetObjects<EntityViewArgument>().FirstOrDefault();
            if (entityView.EntityId == null 
                || !entityView.EntityId.Contains("Entity-")
                || !string.IsNullOrEmpty(entityView.Action)
                || request?.Entity == null
                || !request.Entity.HasComponent<EntityViewComponent>())              
            {
                return Task.FromResult(entityView);
            }

            var viewComponent = request.Entity.GetComponent<EntityViewComponent>();

            if (viewComponent.View.ChildViews.Count == 0)
            {
                entityView.ChildViews.Add(viewComponent.View);
            }
            else
            {
                viewComponent.View.ChildViews.OfType<EntityView>().Where(v => !string.IsNullOrEmpty(v.ItemId) && v.ItemId.Contains("Composer-")).ForEach(v => entityView.ChildViews.Add(v));
            }
            

            return Task.FromResult(entityView);
        }
    }
}
