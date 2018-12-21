namespace Plugin.Sample.Messaging.EntityViews
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Messaging.Entities;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = global::Plugin.Sample.Messaging.Policies.PluginPolicy;

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

            if (entityView.Name != "MessagesDashboard")
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            entityView.UiHint = "Flat";
            entityView.Icon = pluginPolicy.Icon;
            entityView.DisplayName = "Messages";
            entityView.DisplayName = "My Messages";
            
            var messages = await this._commerceCommander.Command<ListCommander>().GetListItems<MessageEntity>(context.CommerceContext, "MessageEntities", 0, 20);

            var newEntityViewTable = new EntityView
            {
                Name = "MessagingView",
                UiHint = "Table",
                Icon = pluginPolicy.Icon,
                ItemId = string.Empty,
            };
            entityView.ChildViews.Add(newEntityViewTable);
            
            foreach (var message in messages)
            {
                newEntityViewTable.ChildViews.Add(
                    new EntityView
                    {
                        ItemId = message.Id,
                        Icon = pluginPolicy.Icon,
                        Properties = new List<ViewProperty> {
                            new ViewProperty {Name = "ItemId", RawValue = message.Id, UiType = "EntityLink", IsHidden = true },
                            new ViewProperty {Name = "Name", RawValue = message.Name, UiType = "EntityLink" },
                            new ViewProperty {Name = "Message", RawValue = message.History.First().EventMessage, OriginalType = "Html", UiType = "Html" }
                        }
                    });
            }

            return entityView;
        }
    }
}
