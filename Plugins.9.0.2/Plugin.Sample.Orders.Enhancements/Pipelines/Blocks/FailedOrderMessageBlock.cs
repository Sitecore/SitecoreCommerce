// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FailedOrderMessageBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   Defines the FailedOrderMessageBlock block.
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
    using Sitecore.Commerce.Plugin.Carts;

    /// <summary>
    ///  Defines the registered plugin block.
    /// </summary>
    [PipelineDisplayName("FailedOrderMessageBlock")]
    public class FailedOrderMessageBlock : PipelineBlock<CartEmailArgument, CartEmailArgument, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedOrderMessageBlock" /> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public FailedOrderMessageBlock(CommerceCommander commerceCommander)
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
        public override async Task<CartEmailArgument> Run(CartEmailArgument arg, CommercePipelineExecutionContext context)
        {
            if (arg == null)
            {
                return arg;
            }

            //Validate the Cart
            foreach(var cartLine in arg.Cart.Lines)
            {
                if (cartLine.HasComponent<ItemAvailabilityComponent>())
                {
                    var itemAvailabilityComponent = cartLine.GetComponent<ItemAvailabilityComponent>();
                    if (!itemAvailabilityComponent.IsAvailable)
                    {
                        if (cartLine.HasComponent<CartProductComponent>())
                        {
                            var cartProductComponent = cartLine.GetComponent<CartProductComponent>();
                            if (!cartProductComponent.HasPolicy<AvailabilityAlwaysPolicy>())
                            {
                                var messageEntity = new Messaging.MessageEntity();
                                messageEntity.Id = Messaging.MessageEntity.IdPrefix<Messaging.MessageEntity>() + Guid.NewGuid().ToString("N");
                                messageEntity.Name = "Order.UnavailableItem";
                                messageEntity.GetComponent<ListMembershipsComponent>().Memberships.Add(CommerceEntity.ListName<Messaging.MessageEntity>());
                                messageEntity.History.Add(new Messaging.HistoryEntryModel { Name = "Order.UnavailableItem", EventMessage = $"An Order was attempted with an unavailable item" });

                                var contactComponent = arg.Cart.GetComponent<ContactComponent>();
                                if (contactComponent.IsRegistered)
                                {

                                }
                                messageEntity.Components.Add(contactComponent);
                                messageEntity.Components.Add(itemAvailabilityComponent);

                                var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, messageEntity);
                            }
                        }
                    }
                }
            }

            if (context.CommerceContext.GetMessages().Any(p => p.Code == "Error"))
            {

                //var countries = await this._commerceCommander.Command<GetCountriesCommand>().Process(context.CommerceContext);



                try
                {

                }
                catch (Exception ex)
                {
                    context.CommerceContext.LogException("DevOps.CheckDeserializedEntityBlock", ex);
                }
            }

            return arg;
        }
    }
}
