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

    [PipelineDisplayName("SuccessOrderMessageBlock")]
    public class SuccessOrderMessageBlock : PipelineBlock<Order, Order, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public SuccessOrderMessageBlock(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<Order> Run(Order order, CommercePipelineExecutionContext context)
        {
            if (order == null)
            {
                return order;
            }

            var messageEntity = new MessageEntity
            {
                Id = CommerceEntity.IdPrefix<MessageEntity>() + Guid.NewGuid().ToString("N"), Name = "Order.Success"
            };
            messageEntity.GetComponent<ListMembershipsComponent>().Memberships.Add(CommerceEntity.ListName<MessageEntity>());
            messageEntity.History.Add(new HistoryEntryModel { Name = messageEntity.Name, EventMessage = $"An Order was completed" });
            var contactComponent = order.GetComponent<ContactComponent>();
            
            messageEntity.Components.Add(contactComponent);
            messageEntity.Models.Add(order.Totals);
            messageEntity.Models.Add(new OrderSummaryModel {
                ConfirmationId = order.OrderConfirmationId, OrderId = order.Id, Status = order.Status
            });

            await this._commerceCommander.PersistEntity(context.CommerceContext, messageEntity);

            return order;
        }
    }
}
