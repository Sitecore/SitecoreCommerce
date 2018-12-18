namespace Plugin.Sample.Orders.Enhancements.Pipelines.Blocks
{
    using System;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Messaging.Entities;
    using global::Plugin.Sample.Messaging.Models;
    using global::Plugin.Sample.Orders.Enhancements.Models;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("PerfStopCalculatingCartOnGet")]
    public class PerfStopCalculatingCartOnGet : PipelineBlock<Order, Order, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public PerfStopCalculatingCartOnGet(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<Order> Run(Order arg, CommercePipelineExecutionContext context)
        {
            if (arg == null)
            {
                return null;
            }

            var messageEntity = new MessageEntity
            {
                Id = CommerceEntity.IdPrefix<MessageEntity>() + Guid.NewGuid().ToString("N"), Name = "tryry"
            };
            messageEntity.GetComponent<ListMembershipsComponent>().Memberships.Add(CommerceEntity.ListName<MessageEntity>());
            messageEntity.History.Add(new HistoryEntryModel { Name = messageEntity.Name, EventMessage = $"An Order was completed" });
            var contactComponent = arg.GetComponent<ContactComponent>();
           
            messageEntity.Components.Add(contactComponent);
            messageEntity.Models.Add(arg.Totals);
            messageEntity.Models.Add(new OrderSummaryModel { ConfirmationId = arg.OrderConfirmationId, OrderId = arg.Id, Status = arg.Status });

            await this._commerceCommander.PersistEntity(context.CommerceContext, messageEntity);
            
            return arg;
        }
    }
}
