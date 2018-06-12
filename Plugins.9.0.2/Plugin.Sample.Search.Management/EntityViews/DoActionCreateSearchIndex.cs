
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Plugin.Sample.Search.Management
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Search;

    /// <summary>
    /// Defines the do action create search index block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("DoActionCreateSearchIndex")]
    public class DoActionCreateSearchIndex : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionCreateSearchIndex"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionCreateSearchIndex(CommerceCommander commerceCommander)
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
                || !entityView.Action.Equals("Search-CreateSearchIndex", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var searchScopePolicy = context.CommerceContext.Environment.GetPolicies<SearchScopePolicy>().FirstOrDefault(p=>p.Name == entityView.ItemId);
                if (searchScopePolicy != null)
                {
                    var name = entityView.Properties.First(p => p.Name == "Name").Value ?? "";
                    var commandResult = await this._commerceCommander.Command<CreateSearchIndexCommand>().Process(context.CommerceContext, name);
                }
                else
                {
                    context.Logger.LogError($"Search.DoActionCreateSearchIndex.NoSearchScopePolicy: Name={entityView.ItemId}");
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Search.DoActionCreateSearchIndex.Exception: Message={ex.Message}");
            }

            return entityView;
        }
   }
}
