
using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace Plugin.Sample.ViewMaster
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Commerce.EntityViews;
    
    using Sitecore.Commerce.Core.Commands;

    /// <summary>
    /// Defines the do action remove line item block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("DoActionClearAllEvents")]
    public class DoActionClearAllEvents : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionClearAllEvents"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionClearAllEvents(CommerceCommander commerceCommander)
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
                || !entityView.Action.Equals("ViewMaster-ClearEvents", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var sessionEntity = await this._commerceCommander.GetEntity<ViewmasterSessionEntity>(context.CommerceContext, "ViewMaster_Session", true);

                sessionEntity.Views.Clear();

                var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, sessionEntity);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionRemoveDashboardEntity.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
