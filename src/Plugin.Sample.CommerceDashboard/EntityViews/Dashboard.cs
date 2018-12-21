namespace Plugin.Sample.CommerceDashboard.EntityViews
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Plugin.Sample.CommerceDashboard.Entities;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
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

            if (entityView.Name != "MyDashboard")
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            entityView.UiHint = "Flat";
            entityView.Icon = pluginPolicy.Icon;
            entityView.DisplayName = "My Dashboard";

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "ViewProperty1",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = "ValueString"
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "HtmlProperty",
                    IsHidden = false,
                    IsReadOnly = true,
                    OriginalType = "Html",
                    UiType = "Html",
                    RawValue = "<b>ValueString</b>"
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "TableSample",
                    IsHidden = false,
                    IsReadOnly = true,
                    OriginalType = "Html",
                    UiType = "Html",
                    RawValue = "<table><tr><td style='color: red;width:100%'>tablecell1</td><td>tablecell2</td></tr><tr><td>tablecell1b</td><td>tablecell2b</td></tr></table>"
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "DivSample",
                    IsHidden = false,
                    IsReadOnly = true,
                    OriginalType = "Html",
                    UiType = "Html",
                    RawValue = "<div style='color: red;'>In Div</div><div style='color: blue;'>In Div2</div>"
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "ImageProperty",
                    IsHidden = false,
                    IsReadOnly = true,
                    OriginalType = "Html",
                    UiType = "Html",
                    RawValue = "<img alt='This is the alternate' height=100 width=100 src='http://sxa.storefront.com/-/media/Images/Habitat/6042177_01.ashx?h=625&w=770' style=''/>"
                });

            var newEntityViewTable = new EntityView
            {
                Name = "Example Table View",
                UiHint = "Table",
                Icon = pluginPolicy.Icon,
                ItemId = string.Empty,
            };
            entityView.ChildViews.Add(newEntityViewTable);

            var sampleDashboardEntities = await this._commerceCommander.Command<ListCommander>()
                .GetListItems<SampleDashboardEntity>(context.CommerceContext,
                    CommerceEntity.ListName<SampleDashboardEntity>(), 0, 99);
            foreach (var sampleDashboardEntity in sampleDashboardEntities)
            {
                newEntityViewTable.ChildViews.Add(
                    new EntityView
                    {
                        ItemId = sampleDashboardEntity.Id,
                        Icon = pluginPolicy.Icon,
                        Properties = new List<ViewProperty> {
                            new ViewProperty {Name = "ItemId", RawValue = sampleDashboardEntity.Id, UiType = "EntityLink" },
                            new ViewProperty {Name = "Name", RawValue = sampleDashboardEntity.Name },
                            new ViewProperty {Name = "DisplayName", RawValue = sampleDashboardEntity.DisplayName, OriginalType = "Html", UiType = "Html" }
                        }
                    });
            }

            return entityView;
        }
    }
}
