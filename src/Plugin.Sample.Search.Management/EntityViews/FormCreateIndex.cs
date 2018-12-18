namespace Plugin.Sample.Search.Management.EntityViews
{
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Search;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("FormCreateIndex")]
    public class FormCreateIndex : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");
            
            if (entityView.Action != "Search-CreateSearchIndex")
            {
                return Task.FromResult(entityView);
            }
            var searchScopePolicy = context.CommerceContext.Environment.GetPolicies<SearchScopePolicy>().FirstOrDefault(p => p.Name == entityView.ItemId);
            if (searchScopePolicy != null)
            {
                entityView.Properties.Add(
                    new ViewProperty
                    {
                        Name = "Name",
                        IsHidden = false,
                        IsRequired = false,
                        RawValue = searchScopePolicy.Name
                    });
            }

            return Task.FromResult(entityView);
        }
    }

}
