namespace Plugin.Sample.Search.Management.EntityViews
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Search;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("DoActionDeleteSearchIndex")]
    public class DoActionDeleteSearchIndex : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionDeleteSearchIndex(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("Search-DeleteSearchIndex", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var searchScopePolicy = context.CommerceContext.Environment.GetPolicies<SearchScopePolicy>().FirstOrDefault(p=>p.Name == entityView.ItemId);
                if (searchScopePolicy != null)
                {
                    await this._commerceCommander.Command<DeleteSearchIndexCommand>().Process(context.CommerceContext, entityView.ItemId);
                }
                else
                {
                    context.Logger.LogError($"Search.DoActionDeleteSearchIndex.NoSearchScopePolicy: Name={entityView.ItemId}");
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Search.DoActionDeleteSearchIndex.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
