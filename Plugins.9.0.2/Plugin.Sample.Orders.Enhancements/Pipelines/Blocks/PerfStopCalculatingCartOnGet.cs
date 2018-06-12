// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PerfStopCalculatingCartOnGet.cs" company="Sitecore Corporation">
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
    [PipelineDisplayName("PerfStopCalculatingCartOnGet")]
    public class PerfStopCalculatingCartOnGet : PipelineBlock<Order, Order, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessOrderMessageBlock" /> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public PerfStopCalculatingCartOnGet(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="arg">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Order"/>
        /// </returns>
        public override async Task<Order> Run(Order arg, CommercePipelineExecutionContext context)
        {
            if (arg == null)
            {
                return arg;
            }
            
            var messageEntity = new Messaging.MessageEntity();
            messageEntity.Id = Messaging.MessageEntity.IdPrefix<Messaging.MessageEntity>() + Guid.NewGuid().ToString("N");
            messageEntity.Name = "tryry";
            messageEntity.GetComponent<ListMembershipsComponent>().Memberships.Add(CommerceEntity.ListName<Messaging.MessageEntity>());
            messageEntity.History.Add(new Messaging.HistoryEntryModel { Name = messageEntity.Name, EventMessage = $"An Order was completed" });
            var contactComponent = arg.GetComponent<ContactComponent>();
            if (contactComponent.IsRegistered)
            {

            }
            messageEntity.Components.Add(contactComponent);
            messageEntity.Models.Add(arg.Totals);
            messageEntity.Models.Add(new OrderSummaryModel { ConfirmationId = arg.OrderConfirmationId, OrderId = arg.Id, Status = arg.Status });


            var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, messageEntity);

            //Validate the Cart
            //foreach (var cartLine in arg.Lines)
            //{
            //    if (cartLine.HasComponent<ItemAvailabilityComponent>())
            //    {
            //        var itemAvailabilityComponent = cartLine.GetComponent<ItemAvailabilityComponent>();
            //        if (!itemAvailabilityComponent.IsAvailable)
            //        {
            //            var messageEntity = new MessageEntity();
            //            messageEntity.Id = MessageEntity.IdPrefix<MessageEntity>() + Guid.NewGuid().ToString("N");
            //            messageEntity.GetComponent<ListMembershipsComponent>().Memberships.Add(CommerceEntity.ListName<MessageEntity>());
            //            messageEntity.History.Add(new HistoryEntryModel { Name = "Order.UnavailableItem", EventMessage = $"An Order was attempted with an unavailable item" });
            //            var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, messageEntity);
            //        }
            //    }
            //}

            //if (context.CommerceContext.GetMessages().Any(p => p.Code == "Error"))
            //{

            //    //var countries = await this._commerceCommander.Command<GetCountriesCommand>().Process(context.CommerceContext);



            //    try
            //    {

            //    }
            //    catch (Exception ex)
            //    {
            //        context.CommerceContext.LogException("DevOps.CheckDeserializedEntityBlock", ex);
            //    }
            //}

            return arg;
        }
    }
}
