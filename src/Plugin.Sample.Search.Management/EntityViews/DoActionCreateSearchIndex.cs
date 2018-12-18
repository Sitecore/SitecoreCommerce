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

    [PipelineDisplayName("DoActionCreateSearchIndex")]
    public class DoActionCreateSearchIndex : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionCreateSearchIndex(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

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
                    await this._commerceCommander.Command<CreateSearchIndexCommand>().Process(context.CommerceContext, name);
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
