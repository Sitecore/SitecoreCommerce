// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FailUnavailableItemAdded.cs" company="Sitecore Corporation">
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
    using Sitecore.Commerce.Plugin.Carts;

    /// <summary>
    ///  Defines the registered plugin block.
    /// </summary>
    [PipelineDisplayName("FailUnavailableItemAdded")]
    public class FailUnavailableItemAdded : PipelineBlock<Cart, Cart, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="FailUnavailableItemAdded" /> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public FailUnavailableItemAdded(CommerceCommander commerceCommander)
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
        public override async Task<Cart> Run(Cart arg, CommercePipelineExecutionContext context)
        {
            if (arg == null)
            {
                return arg;
            }
            
            //Validate the Cart
            foreach (var cartLine in arg.Lines)
            {
                if (cartLine.HasComponent<ItemAvailabilityComponent>())
                {
                    var itemAvailabilityComponent = cartLine.GetComponent<ItemAvailabilityComponent>();
                    if (!itemAvailabilityComponent.IsAvailable)
                    {
                        var messageEntity = new Messaging.MessageEntity();
                        messageEntity.Id = Messaging.MessageEntity.IdPrefix<Messaging.MessageEntity>() + Guid.NewGuid().ToString("N");
                        messageEntity.Name = "Cart.ItemUnavailable";
                        messageEntity.GetComponent<ListMembershipsComponent>().Memberships.Add(CommerceEntity.ListName<Messaging.MessageEntity>());
                        messageEntity.History.Add(new Messaging.HistoryEntryModel { Name = "Cart.UnavailableItem", EventMessage = $"An Item as added to the Cart that is not Available" });
                        messageEntity.Components.Add(itemAvailabilityComponent);
                        var contactComponent = arg.GetComponent<ContactComponent>();
                        if (!contactComponent.IsRegistered)
                        {

                        }
                        messageEntity.Components.Add(contactComponent);
                        messageEntity.Models.Add(new CartSummaryModel { CartId = arg.Id });
                        var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, messageEntity);
                    }
                }
            }
            return arg;
        }
    }
}
