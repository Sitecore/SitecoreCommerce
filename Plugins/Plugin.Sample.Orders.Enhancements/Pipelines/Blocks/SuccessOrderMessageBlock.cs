// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SuccessOrderMessageBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   Defines the SuccessOrderMessageBlock block.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Orders.Enhancements
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;
    using Newtonsoft.Json.Linq;
    using System;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Commerce.Plugin.Availability;
    using Sitecore.Commerce.Plugin.ManagedLists;

    /// <summary>
    ///  Defines the registered plugin block.
    /// </summary>
    [PipelineDisplayName("SuccessOrderMessageBlock")]
    public class SuccessOrderMessageBlock : PipelineBlock<Order, Order, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessOrderMessageBlock" /> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public SuccessOrderMessageBlock(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Order"/>
        /// </returns>
        public override async Task<Order> Run(Order order, CommercePipelineExecutionContext context)
        {
            if (order == null)
            {
                return order;
            }
            
            var messageEntity = new Messaging.MessageEntity();
            messageEntity.Id = Messaging.MessageEntity.IdPrefix<Messaging.MessageEntity>() + Guid.NewGuid().ToString("N");
            messageEntity.Name = "Order.Success";
            messageEntity.GetComponent<ListMembershipsComponent>().Memberships.Add(CommerceEntity.ListName<Messaging.MessageEntity>());
            messageEntity.History.Add(new Messaging.HistoryEntryModel { Name = messageEntity.Name, EventMessage = $"An Order was completed" });
            var contactComponent = order.GetComponent<ContactComponent>();
            if (contactComponent.IsRegistered)
            {

            }
            messageEntity.Components.Add(contactComponent);
            messageEntity.Models.Add(order.Totals);
            messageEntity.Models.Add(new OrderSummaryModel {
                ConfirmationId = order.OrderConfirmationId, OrderId = order.Id, Status = order.Status
            });

            var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, messageEntity);

            return order;
        }
    }
}
