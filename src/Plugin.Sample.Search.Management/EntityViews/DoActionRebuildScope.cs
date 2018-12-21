namespace Plugin.Sample.Search.Management.EntityViews
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Search;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionRebuildScope")]
    public class DoActionRebuildScope : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionRebuildScope(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
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
                    await this._commerceCommander.Command<RunMinionCommand>().Process(context.CommerceContext,
                        "Sitecore.Commerce.Plugin.Search.FullIndexMinion, Sitecore.Commerce.Plugin.Search",
                        "HabitatMinions", 
                        new List<Policy>
                        {
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
