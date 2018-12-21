namespace Plugin.Sample.ListMaster.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using global::Plugin.Sample.ListMaster.Entities;
    using global::Plugin.Sample.ListMaster.Models;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionClearAllLists")]
    public class DoActionClearAllLists : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionClearAllLists(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("ListMaster-ClearLists", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var listTrackingEntity = await this._commerceCommander.GetEntity<ListTrackingEntity>(context.CommerceContext, "ListMaster-ListTracker", true)
                    ?? new ListTrackingEntity("ListMaster-ListTracker");
                if (!listTrackingEntity.IsPersisted)
                {
                    listTrackingEntity.Id = "ListMaster-ListTracker";
                }
                listTrackingEntity.Lists = new System.Collections.Generic.List<TrackedList>();

                await this._commerceCommander.PersistEntity(context.CommerceContext, listTrackingEntity);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionAddList.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
