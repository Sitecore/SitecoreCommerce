namespace Plugin.Sample.Orders.Enhancements.Pipelines.Blocks
{
    using System;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Messaging.Entities;
    using global::Plugin.Sample.Messaging.Models;
    using global::Plugin.Sample.Orders.Enhancements.Models;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Availability;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("FailUnavailableItemAdded")]
    public class FailUnavailableItemAdded : PipelineBlock<Cart, Cart, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public FailUnavailableItemAdded(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<Cart> Run(Cart arg, CommercePipelineExecutionContext context)
        {
            if (arg == null)
            {
                return null;
            }
            
            foreach (var cartLine in arg.Lines)
            {
                if (cartLine.HasComponent<ItemAvailabilityComponent>())
                {
                    var itemAvailabilityComponent = cartLine.GetComponent<ItemAvailabilityComponent>();
                    if (!itemAvailabilityComponent.IsAvailable)
                    {
                        var messageEntity = new MessageEntity
                        {
                            Id = CommerceEntity.IdPrefix<MessageEntity>() + Guid.NewGuid().ToString("N"),
                            Name = "Cart.ItemUnavailable"
                        };
                        messageEntity.GetComponent<ListMembershipsComponent>().Memberships.Add(CommerceEntity.ListName<MessageEntity>());
                        messageEntity.History.Add(new HistoryEntryModel { Name = "Cart.UnavailableItem", EventMessage = $"An Item as added to the Cart that is not Available" });
                        messageEntity.Components.Add(itemAvailabilityComponent);
                        var contactComponent = arg.GetComponent<ContactComponent>();
                        
                        messageEntity.Components.Add(contactComponent);
                        messageEntity.Models.Add(new CartSummaryModel { CartId = arg.Id });
                        await this._commerceCommander.PersistEntity(context.CommerceContext, messageEntity);
                    }
                }
            }
            return arg;
        }
    }
}
