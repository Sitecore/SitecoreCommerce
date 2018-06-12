
using System.Threading.Tasks;

namespace Plugin.Sample.Search.Management
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Search.Solr;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which populates an EntityView.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("Dashboard")]
    public class Dashboard : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {

        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dashboard"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public Dashboard(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
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

            //TODO
            //entityView.Properties.Add(
            //    new ViewProperty
            //    {
            //        Name = "UniqueIdFieldName",
            //        IsHidden = false,
            //        IsReadOnly = true,
            //        RawValue = solrPolicy.UniqueIdFieldName
            //    });

            return Task.FromResult(entityView);
        }
    }
}
