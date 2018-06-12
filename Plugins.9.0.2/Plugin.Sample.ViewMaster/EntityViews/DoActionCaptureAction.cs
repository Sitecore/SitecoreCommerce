
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.ViewMaster
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
    public class DoActionCaptureAction : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionCaptureAction"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionCaptureAction(CommerceCommander commerceCommander)
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
                || !entityView.Action.Equals("MyDashboard-AddDashboardEntity", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {

                var sessionEntity = await this._commerceCommander.GetEntity<ViewmasterSessionEntity>(context.CommerceContext, "ViewMaster_Session", true);
                if (!sessionEntity.IsPersisted)
                {
                    sessionEntity.Id = "ViewMaster_Session";
                }

                var viewRecord = new ViewMasterViewRecord();
                viewRecord.View = entityView;
                foreach (var header in context.CommerceContext.Headers)
                {
                    viewRecord.Headers.Add(header.Key, header.Value);
                }
                sessionEntity.Views.Add(viewRecord);

                //var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, sessionEntity);

            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionAddDashboardEntity.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
