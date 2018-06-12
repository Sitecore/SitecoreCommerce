
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Plugin.Sample.ListMaster
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Commerce.Plugin.SQL;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System.Linq;


    /// <summary>
    /// Defines a block which populates an EntityView for a Sample Page in the Sitecore Commerce Focused Commerce Experience.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("ViewManagedList")]
    public class ViewManagedList : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {

        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewManagedList"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public ViewManagedList(CommerceCommander commerceCommander)
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

            if (!entityView.EntityId.Contains("Entity-ManagedList-"))
            {
                return entityView;
            }

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            //entityView.UiHint = "Table";
            //entityView.Icon = pluginPolicy.Icon;
            //entityView.DisplayName = "List Master";


            var managedList = entityViewArgument.Entity as ManagedList;
            if (managedList != null)
            {
                var historyComponent = managedList.GetComponent<Messaging.HistoryComponent>();
                if (historyComponent.History.Count > 0)
                {
                    var historyEntityView = new EntityView
                    {
                        Name = "Publish Messages",
                        UiHint = "Table",
                        Icon = pluginPolicy.Icon,
                        ItemId = "",
                    };
                    entityView.ChildViews.Add(historyEntityView);

                    foreach (var historyEntry in historyComponent.History)
                    {
                        var listCount = await this._commerceCommander.Command<GetListCountCommand>().Process(context.CommerceContext, managedList.Name);

                        historyEntityView.ChildViews.Add(
                            new EntityView
                            {
                                ItemId = managedList.Id,
                                Icon = pluginPolicy.Icon,
                                Properties = new List<ViewProperty> {
                            //new ViewProperty {Name = "ItemId", RawValue = managedList.Id, UiType = "EntityLink" },
                            new ViewProperty {Name = "Date", RawValue = historyEntry.EventDate.ToString("yyyy-MMM-dd hh:mm:ss")},
                            new ViewProperty {Name = "Message", RawValue = historyEntry.EventMessage}
                                }
                            }
                        );
                    }
                }
                
            }

            return entityView;
        }
    }
}
