namespace Plugin.Sample.Messaging.EntityViews
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Messaging.Entities;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionClearMessages")]
    public class DoActionClearMessages : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionClearMessages(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

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
                        await this._commerceCommander.Command<DeleteEntityCommand>().Process(context.CommerceContext, message.Id);
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
