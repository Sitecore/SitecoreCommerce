namespace Plugin.Sample.ViewMaster.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using global::Plugin.Sample.ViewMaster.Entities;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionAddDashboardEntity")]
    public class DoActionCaptureAction : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionCaptureAction(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("MyDashboard-AddDashboardEntity", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var sessionEntity = await this._commerceCommander.GetEntity<ViewMasterSessionEntity>(context.CommerceContext, "ViewMaster_Session", true);
                if (!sessionEntity.IsPersisted)
                {
                    sessionEntity.Id = "ViewMaster_Session";
                }

                var viewRecord = new ViewMasterViewRecord { View = entityView };
                foreach (var header in context.CommerceContext.Headers)
                {
                    viewRecord.Headers.Add(header.Key, header.Value);
                }
                sessionEntity.Views.Add(viewRecord);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionAddDashboardEntity.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
