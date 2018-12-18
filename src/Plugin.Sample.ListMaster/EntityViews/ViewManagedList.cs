namespace Plugin.Sample.ListMaster.EntityViews
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Messaging.Components;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Commerce.Plugin.SQL;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = global::Plugin.Sample.ListMaster.Policies.PluginPolicy;

    [PipelineDisplayName("ViewManagedList")]
    public class ViewManagedList : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public ViewManagedList(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (!entityView.EntityId.Contains("Entity-ManagedList-"))
            {
                return entityView;
            }

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            if (entityViewArgument.Entity is ManagedList managedList)
            {
                var historyComponent = managedList.GetComponent<HistoryComponent>();
                if (historyComponent.History.Count > 0)
                {
                    var historyEntityView = new EntityView
                    {
                        Name = "Publish Messages",
                        UiHint = "Table",
                        Icon = pluginPolicy.Icon,
                        ItemId = string.Empty,
                    };
                    entityView.ChildViews.Add(historyEntityView);

                    foreach (var historyEntry in historyComponent.History)
                    {
                        await this._commerceCommander.Command<GetListCountCommand>().Process(context.CommerceContext, managedList.Name);

                        historyEntityView.ChildViews.Add(
                            new EntityView
                            {
                                ItemId = managedList.Id,
                                Icon = pluginPolicy.Icon,
                                Properties = new List<ViewProperty>
                                {
                                    new ViewProperty {Name = "Date", RawValue = historyEntry.EventDate.ToString("yyyy-MMM-dd hh:mm:ss")},
                                    new ViewProperty {Name = "Message", RawValue = historyEntry.EventMessage}
                                }
                            });
                    }
                }
            }

            return entityView;
        }
    }
}
