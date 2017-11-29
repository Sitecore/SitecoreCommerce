
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Plugin.Sample.Search.Management
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Core.Commands;
    using System.Collections.Generic;
    using Sitecore.Commerce.Plugin.Search;
    using Sitecore.Commerce.Plugin.BusinessUsers;

    /// <summary>
    /// Defines the do action DoActionRebuildScope block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("DoActionRebuildScope")]
    public class DoActionRebuildScope : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionRebuildScope"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionRebuildScope(CommerceCommander commerceCommander)
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

            if (entityView == null
                || !entityView.Action.Equals("Search-RebuildScope", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var searchScopePolicy = context.CommerceContext.Environment.GetPolicies<SearchScopePolicy>().FirstOrDefault(p=>p.Name == entityView.ItemId);
                if (searchScopePolicy != null)
                {
                    var commandResult = await this._commerceCommander.Command<RunMinionCommand>()
                    .Process(context.CommerceContext,
                    "Sitecore.Commerce.Plugin.Search.FullIndexMinion, Sitecore.Commerce.Plugin.Search",
                    "HabitatMinions", new List<Policy> {
                     new RunMinionPolicy { WithListToWatch = searchScopePolicy.FullListName }
                     });
                }
                else
                {
                    context.Logger.LogError($"Search.DoActionRebuildScope.NoSearchScopePolicy: Name={entityView.ItemId}");
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Search.DoActionRebuildScope.Exception: Message={ex.Message}");
            }

            return entityView;
        }
   }
}
