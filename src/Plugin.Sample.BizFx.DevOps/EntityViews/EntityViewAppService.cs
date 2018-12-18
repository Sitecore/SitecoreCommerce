namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System.Linq;
    using System.Threading.Tasks;

    using Plugin.Sample.BizFx.DevOps.Entities;
    using Plugin.Sample.JsonCommander.Components;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = Plugin.Sample.BizFx.DevOps.Policies.PluginPolicy;

    [PipelineDisplayName("EntityViewAppService")]
    public class EntityViewAppService : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public EntityViewAppService(CommerceCommander commerceCommander)
        {
            _commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null)
            {
                return null;
            }

            if (entityView.EntityId == null || !entityView.EntityId.Contains("Entity-AppService-"))
            {
                return entityView;
            }

            var entityViewArgument = _commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            entityView.UiHint = "Flat";
            entityView.Icon = pluginPolicy.Icon;

            var pathsView = new EntityView
            {
                EntityId = string.Empty,
                ItemId = "Id",
                DisplayName = "Messages",
                Name = "Messages",
                UiHint = "Table"
            };

            foreach (var message in entityViewArgument.Entity.GetComponent<ActionHistoryComponent>().History)
            {
                var pathsView1 = new EntityView
                {
                    EntityId = string.Empty,
                    ItemId = "Id",
                    DisplayName = "Connection Paths",
                    Name = "Connection Paths",
                    UiHint = string.Empty
                };
                pathsView.ChildViews.Add(pathsView1);

                pathsView1.Properties
                    .Add(new ViewProperty { Name = "Name", RawValue = message.Name });
                pathsView1.Properties
                    .Add(new ViewProperty { Name = "Date", RawValue = message.Completed });
                pathsView1.Properties
                    .Add(new ViewProperty { Name = "Response", RawValue = message.Response });
                pathsView1.Properties
                    .Add(new ViewProperty { Name = "EntityId", RawValue = message.EntityId });
                pathsView1.Properties
                    .Add(new ViewProperty { Name = "ItemId", RawValue = message.ItemId });

                if (message.Response != "Ok")
                {
                    pathsView1.Properties
                        .Add(new ViewProperty { Name = "JSON", RawValue = message.JSON });
                }
            }

            entityView.ChildViews.Add(pathsView);

            await _commerceCommander.Command<ListCommander>()
                .GetListItems<AppService>(context.CommerceContext, "AppServices", 0, 999);

            return entityView;
        }
    }
}
