namespace Plugin.Sample.ListMaster.EntityViews
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using global::Plugin.Sample.ListMaster.Components;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Commerce.Plugin.SQL;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = global::Plugin.Sample.ListMaster.Policies.PluginPolicy;

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

            if (entityView.Name != "ListMaster")
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            entityView.UiHint = "Table";
            entityView.Icon = pluginPolicy.Icon;
            entityView.DisplayName = "List Master";
            
            var arg = new FindEntitiesInListArgument(typeof(CommerceEntity), "ManagedLists", 0, 100);
            var managedLists = await this._commerceCommander.Pipeline<FindEntitiesInListPipeline>().Run(arg, context.CommerceContext.GetPipelineContextOptions());

            if (managedLists.List.TotalItemCount > 0)
            {
                foreach (var managedList in managedLists.List.Items.OfType<ManagedList>())
                {
                    managedList.TotalItemCount = await this._commerceCommander.Command<GetListCountCommand>().Process(context.CommerceContext, managedList.Name);

                    var publishedListEntities = managedList.GetComponent<ListEntitiesInPublish>();

                    var duration = publishedListEntities.LastPublishEnd - publishedListEntities.LastPublishStart;

                    entityView.ChildViews.Add(
                        new EntityView
                        {
                            ItemId = managedList.Id,
                            Icon = pluginPolicy.Icon,
                            Properties = new List<ViewProperty>
                            {
                                new ViewProperty {Name = "ItemId", RawValue = managedList.Id, UiType = "EntityLink", IsHidden = true },
                                new ViewProperty {Name = "Name", RawValue = managedList.Name, UiType = "EntityLink" },
                                new ViewProperty {Name = "DisplayName", RawValue = managedList.DisplayName},
                                new ViewProperty {Name = "Count", RawValue = managedList.TotalItemCount},
                                new ViewProperty {Name = "LastPublishStart", RawValue = publishedListEntities.LastPublishStart.ToString("yyyy-MMM-dd hh:mm zzz")},
                                new ViewProperty {Name = "LastPublishEnd", RawValue = publishedListEntities.LastPublishEnd.ToString("yyyy-MMM-dd hh:mm zzz")},
                                new ViewProperty {Name = "LastPublishCount", RawValue = publishedListEntities.LastPublishCount},
                                new ViewProperty {Name = "Duration", DisplayName = "Duration (s)", RawValue = duration.Seconds},
                                new ViewProperty {Name = "LastRow", RawValue = publishedListEntities.LastRow}
                            }
                        });
                }
            }

            return entityView;
        }
    }
}
