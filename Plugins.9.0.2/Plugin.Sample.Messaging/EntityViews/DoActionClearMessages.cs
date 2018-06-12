
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Plugin.Sample.Messaging
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
    [PipelineDisplayName("DoActionClearMessages")]
    public class DoActionClearMessages : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionClearMessages"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionClearMessages(CommerceCommander commerceCommander)
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
                || !entityView.Action.Equals("MessagesDashboard-ClearMessages", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var messages = await this._commerceCommander.Command<ListCommander>().GetListItems<MessageEntity>(context.CommerceContext, "MessageEntities", 0, 100);
                while (messages.ToList().Count > 0)
                {
                    foreach (var message in messages)
                    {
                        var deleteResult = await this._commerceCommander.Command<DeleteEntityCommand>().Process(context.CommerceContext, message.Id);
                    }
                    messages = await this._commerceCommander.Command<ListCommander>().GetListItems<MessageEntity>(context.CommerceContext, "MessageEntities", 0, 100);

                }


            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionRemoveDashboardEntity.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
