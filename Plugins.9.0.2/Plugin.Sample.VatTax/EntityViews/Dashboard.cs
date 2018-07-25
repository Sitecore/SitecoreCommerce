
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Plugin.Sample.VatTax
{
    
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    

    /// <summary>
    /// Defines a block which populates an EntityView for a Sample Page in the Sitecore Commerce Focused Commerce Experience.
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
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "VatTaxDashboard")
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            //entityView.UiHint = "Flat";
            //entityView.Icon = pluginPolicy.Icon;
            //entityView.DisplayName = "My Dashboard";

            var newEntityViewTable = entityView;
            //{
            //    Name = "Example Table View",
            entityView.UiHint = "Table";
            entityView.Icon = pluginPolicy.Icon;
            entityView.ItemId = "";
            //};
            //entityView.ChildViews.Add(newEntityViewTable);

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
                        Properties = new List<ViewProperty> {
                            //new ViewProperty {Name = "ItemId", RawValue = sampleDashboardEntity.Id, UiType = "EntityLink" },
                            //new ViewProperty {Name = "Name", RawValue = sampleDashboardEntity.Name },
                            new ViewProperty {Name = "TaxTag", RawValue = sampleDashboardEntity.TaxTag },
                            new ViewProperty {Name = "CountryCode", RawValue = sampleDashboardEntity.CountryCode },
                            new ViewProperty {Name = "TaxPct", RawValue = sampleDashboardEntity.TaxPct }
                        }
                    }
                );
            }

            return entityView;
        }
    }
}
