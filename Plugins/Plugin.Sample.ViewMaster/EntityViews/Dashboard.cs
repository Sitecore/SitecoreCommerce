
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Plugin.Sample.ViewMaster
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
                ItemId = "",
            };
            entityView.ChildViews.Add(newEntityViewTable);

            var sessionEntity = await this._commerceCommander.GetEntity<ViewmasterSessionEntity>(context.CommerceContext, "ViewMaster_Session", true);

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
                    }
                );
            }

            return entityView;
        }
    }
}
