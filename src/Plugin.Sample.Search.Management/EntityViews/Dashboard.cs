
namespace Plugin.Sample.Search.Management.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Search.Solr;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = global::Plugin.Sample.Search.Management.Policies.PluginPolicy;

    [PipelineDisplayName("Dashboard")]
    public class Dashboard : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "SearchDashboard")
            {
                return Task.FromResult(entityView);
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            entityView.UiHint = "Flat";
            entityView.Icon = pluginPolicy.Icon;
            entityView.DisplayName = "Search Dashboard";
            
            var solrPolicy = context.GetPolicy<SolrSearchPolicy>();

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Solr",
                    IsHidden = false,
                    IsReadOnly = true,
                    OriginalType = "Html",
                    UiType = "Html",
                    RawValue = "<img class='resizeable-solr-logo' alt='Solr Logo' height='75' width='150' src='https://lucene.apache.org/solr/assets/identity/Solr_Logo_on_white.png'>"
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Timeout",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = solrPolicy.ConnectionTimeout
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "DefaultSearchOnlyStringFields",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = solrPolicy.DefaultSearchOnlyStringFields
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "DateTimeFormatString",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = solrPolicy.SolrDateTimeFormatString
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "SolrUrl",
                    IsHidden = false,
                    IsReadOnly = true,
                    UiType = "Html",
                    OriginalType = "Html",
                    RawValue = $"<a href='{solrPolicy.SolrUrl}' target='_blank'>{solrPolicy.SolrUrl}</a> "
                });

            return Task.FromResult(entityView);
        }
    }
}
