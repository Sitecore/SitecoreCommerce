
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.ListMaster
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Commerce.EntityViews;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Plugin.ManagedLists;

    /// <summary>
    /// Defines the do action edit line item block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("DoActionAddDashboardEntity")]
    public class DoActionAddList : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionAddList"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionAddList(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="entityView">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="EntityView"/>.
        /// </returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {

            if (entityView == null
                || !entityView.Action.Equals("ListMaster-AddList", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var list = entityView.Properties.First(p => p.Name == "List").Value ?? "";
                var asManagedList = System.Convert.ToBoolean(entityView.Properties.First(p => p.Name == "AsManagedList").Value);

                //var displayName = entityView.Properties.First(p => p.Name == "DisplayName").Value ?? "";

                //var listTrackingEntity = await this._commerceCommander.GetEntity<ListTrackingEntity>(context.CommerceContext, "ListMaster-ListTracker", true);
                //if (!listTrackingEntity.IsPersisted)
                //{
                //    listTrackingEntity.Id = "ListMaster-ListTracker";
                //}

                //if (listTrackingEntity.Lists == null)
                //{
                //    listTrackingEntity.Lists = new System.Collections.Generic.List<TrackedList>();
                //}
                //listTrackingEntity.Lists.Add(new TrackedList() {Name = list });

                //sampleDashboardEntity.GetComponent<ListMembershipsComponent>().Memberships.Add(CommerceEntity.ListName<SampleDashboardEntity>());

                //var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, listTrackingEntity);

                var listId = "";
                if (asManagedList)
                {
                    listId = $"Entity-ManagedList-{list}";
                }
                else
                {
                    listId = list;
                }

                var managedList = new ManagedList() { Id = listId, Name = list, DisplayName = list };
                managedList.GetComponent<ListMembershipsComponent>().Memberships.Add("ManagedLists");
                var persistManagedListResult = await this._commerceCommander.PersistEntity(context.CommerceContext, managedList);

            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionAddList.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
