namespace Plugin.Sample.ViewMaster.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using global::Plugin.Sample.ViewMaster.Entities;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("DoActionClearAllEvents")]
    public class DoActionClearAllEvents : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionClearAllEvents(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("ViewMaster-ClearEvents", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var sessionEntity = await this._commerceCommander.GetEntity<ViewMasterSessionEntity>(context.CommerceContext, "ViewMaster_Session", true);

                sessionEntity.Views.Clear();

                await this._commerceCommander.PersistEntity(context.CommerceContext, sessionEntity);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionRemoveDashboardEntity.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
