// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionClearTemplatesBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Composer
{
    using System;
    using System.Threading.Tasks;
    

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines the do action clear templates block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(ComposerConstants.Pipelines.Blocks.DoActionClearTemplatesBlock)]
    public class DoActionClearTemplatesBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly ListCommander _commander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionClearTemplatesBlock"/> class.
        /// </summary>
        /// <param name="commander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionClearTemplatesBlock(ListCommander commander)
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
                || !entityView.Action.Equals(context.GetPolicy<KnownComposerActionsPolicy>().ClearTemplates, StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            var templateIds = await this._commander
                .GetListItemIds<ComposerTemplate>(context.CommerceContext, CommerceEntity.ListName<ComposerTemplate>(), 0, int.MaxValue);
            var command = this._commander.Command<DeleteEntityCommand>();
            foreach (var id in templateIds)
            {
                await command.Process(context.CommerceContext, id);
            }

            return entityView;
        }
    }
}
