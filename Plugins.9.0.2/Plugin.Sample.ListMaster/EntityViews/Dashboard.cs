
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
    using System;
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

            if (entityView.Name != "ListMaster")
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            entityView.UiHint = "Table";
            entityView.Icon = pluginPolicy.Icon;
            entityView.DisplayName = "List Master";

            var listTrackingEntity = await this._commerceCommander
                .GetEntity<ListTrackingEntity>(context.CommerceContext, "ListMaster-ListTracker", true);

            var arg = new FindEntitiesInListArgument(typeof(CommerceEntity), "ManagedLists", 0, 100);
            var managedLists = await this._commerceCommander.Pipeline<FindEntitiesInListPipeline>().Run(arg, context.CommerceContext.GetPipelineContextOptions());

            if (managedLists.List.TotalItemCount > 0)
            {
                foreach(var managedList in managedLists.List.Items.OfType<ManagedList>())
                {
                    managedList.TotalItemCount = await this._commerceCommander.Command<GetListCountCommand>().Process(context.CommerceContext, managedList.Name);
                    //var persistedManagedList = await this._commerceCommander.Command<GetManagedListCommand>().Process(context.CommerceContext, managedList.Name);

                    var publishedListEntities = managedList.GetComponent<ListEntitiesInPublish>();


                    var duration = publishedListEntities.LastPublishEnd - publishedListEntities.LastPublishStart;

                    entityView.ChildViews.Add(
                        new EntityView
                        {
                            ItemId = managedList.Id,
                            Icon = pluginPolicy.Icon,
                            Properties = new List<ViewProperty> {
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
                        }
                    );
                }
            }

            return entityView;
        }
    }
}
