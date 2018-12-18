
namespace Plugin.Sample.Orders.Enhancements.Pipelines.Blocks
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Messaging.Entities;
    using global::Plugin.Sample.Messaging.Models;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Availability;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("FailedOrderMessageBlock")]
    public class FailedOrderMessageBlock : PipelineBlock<CartEmailArgument, CartEmailArgument, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public FailedOrderMessageBlock(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
    
        public override async Task<CartEmailArgument> Run(CartEmailArgument arg, CommercePipelineExecutionContext context)
        {
            if (arg == null)
            {
                return null;
            }
            
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
                                var messageEntity = new MessageEntity
                                {
                                    Id = CommerceEntity.IdPrefix<MessageEntity>() + Guid.NewGuid().ToString("N"),
                                    Name = "Order.UnavailableItem"
                                };
                                messageEntity.GetComponent<ListMembershipsComponent>().Memberships.Add(CommerceEntity.ListName<MessageEntity>());
                                messageEntity.History.Add(new HistoryEntryModel { Name = "Order.UnavailableItem", EventMessage = $"An Order was attempted with an unavailable item" });

                                var contactComponent = arg.Cart.GetComponent<ContactComponent>();
                                
                                messageEntity.Components.Add(contactComponent);
                                messageEntity.Components.Add(itemAvailabilityComponent);

                                await this._commerceCommander.PersistEntity(context.CommerceContext, messageEntity);
                            }
                        }
                    }
                }
            }

            if (context.CommerceContext.GetMessages().Any(p => p.Code == "Error"))
            {
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
