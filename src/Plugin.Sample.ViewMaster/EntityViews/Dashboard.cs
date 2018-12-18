namespace Plugin.Sample.ViewMaster.EntityViews
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using global::Plugin.Sample.ViewMaster.Entities;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = global::Plugin.Sample.ViewMaster.Policies.PluginPolicy;

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

            if (entityView.Name != "ViewMaster")
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            entityView.UiHint = "Flat";
            entityView.Icon = pluginPolicy.Icon;
            entityView.DisplayName = "ViewMaster Dashboard";

            var newEntityViewTable = new EntityView
            {
                Name = "Views Recorded",
                UiHint = "Table",
                Icon = pluginPolicy.Icon,
                ItemId = string.Empty,
            };
            entityView.ChildViews.Add(newEntityViewTable);

            var sessionEntity = await this._commerceCommander.GetEntity<ViewMasterSessionEntity>(context.CommerceContext, "ViewMaster_Session", true);

            foreach (var recordedView in sessionEntity.Views)
            {
                newEntityViewTable.ChildViews.Add(
                    new EntityView
                    {
                        ItemId = $"ViewMaster_Session/{recordedView.Id}",
                        Icon = pluginPolicy.Icon,
                        Properties = new List<ViewProperty> {
                            new ViewProperty {Name = "ItemId", RawValue = $"ViewMaster_Session/{recordedView.Id}", UiType = "EntityLink", IsHidden = true },
                            new ViewProperty {Name = "Date", RawValue = recordedView.DateCreated },
                            new ViewProperty {Name = "Name", RawValue = recordedView.View.Name, UiType = "EntityLink" },
                            new ViewProperty {Name = "Entity", RawValue = recordedView.View.EntityId },
                            new ViewProperty {Name = "Item", RawValue = recordedView.View.ItemId },
                            new ViewProperty {Name = "Action", RawValue = recordedView.View.Action },
                            new ViewProperty {Name = "UiHint", RawValue = recordedView.View.UiHint },
                            new ViewProperty {Name = "DisplayName", RawValue = recordedView.View.DisplayName, OriginalType = "Html", UiType = "Html" }
                        }
                    });
            }

            return entityView;
        }
    }
}
