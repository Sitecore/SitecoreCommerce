namespace Plugin.Sample.VatTax.EntityViews
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Plugin.Sample.VatTax.Entities;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = Plugin.Sample.VatTax.Policies.PluginPolicy;

    [PipelineDisplayName("Dashboard")]
    public class Dashboard : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public Dashboard(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "VatTaxDashboard")
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();
            
            var newEntityViewTable = entityView;
            entityView.UiHint = "Table";
            entityView.Icon = pluginPolicy.Icon;
            entityView.ItemId = string.Empty;

            var sampleDashboardEntities = await this._commerceCommander.Command<ListCommander>()
                .GetListItems<VatTaxTableEntity>(context.CommerceContext,
                    CommerceEntity.ListName<VatTaxTableEntity>(), 0, 99);
            foreach (var sampleDashboardEntity in sampleDashboardEntities)
            {
                newEntityViewTable.ChildViews.Add(
                    new EntityView
                    {
                        ItemId = sampleDashboardEntity.Id,
                        Icon = pluginPolicy.Icon,
                        Properties = new List<ViewProperty>
                        {
                            new ViewProperty {Name = "TaxTag", RawValue = sampleDashboardEntity.TaxTag },
                            new ViewProperty {Name = "CountryCode", RawValue = sampleDashboardEntity.CountryCode },
                            new ViewProperty {Name = "TaxPct", RawValue = sampleDashboardEntity.TaxPct }
                        }
                    });
            }

            return entityView;
        }
    }
}
