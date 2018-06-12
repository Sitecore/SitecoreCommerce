
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
    [PipelineDisplayName("DoActionClearAllLists")]
    public class DoActionClearAllLists : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionClearAllLists"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionClearAllLists(CommerceCommander commerceCommander)
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
                || !entityView.Action.Equals("ListMaster-ClearLists", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {

                var listTrackingEntity = await this._commerceCommander.GetEntity<ListTrackingEntity>(context.CommerceContext, "ListMaster-ListTracker", true);
                if (listTrackingEntity == null)
                {
                    listTrackingEntity = new ListTrackingEntity("ListMaster-ListTracker");
                }
                if (!listTrackingEntity.IsPersisted)
                {
                    listTrackingEntity.Id = "ListMaster-ListTracker";
                }
                listTrackingEntity.Lists = new System.Collections.Generic.List<TrackedList>();

                var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, listTrackingEntity);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionAddList.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
