namespace Plugin.Sample.Orders.Enhancements.EntityViews
{
    using System.Linq;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Messaging.Entities;
    using global::Plugin.Sample.Orders.Enhancements.Models;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Availability;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("ViewMessagesDashboard")]
    public class ViewMessagesDashboard : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");
            
            if (entityView.Name != "MessagesDashboard")
            {
                return Task.FromResult(entityView);
            }

            var messagingView = entityView.ChildViews.FirstOrDefault(p => p.Name == "MessagingView");
            foreach(var childView in (messagingView as EntityView).ChildViews.OfType<EntityView>())
            {
                var foundEntity = context.CommerceContext.GetObjects<FoundEntity>().FirstOrDefault(p => p.EntityId == childView.ItemId);
                if (foundEntity != null)
                {
                    var messageEntity = foundEntity.Entity as MessageEntity;
                    var messageProperty = childView.Properties.FirstOrDefault(p => p.Name == "Message");

                    if (messageEntity.HasComponent<ContactComponent>())
                    {
                        var contactComponent = messageEntity.GetComponent<ContactComponent>();
                        messageProperty.RawValue = messageProperty.RawValue + $"<div>ShopperId={contactComponent.ShopperId}|CustomerId=<a href='https://localhost:4200/entityView/Master/{contactComponent.CustomerId}'>{contactComponent.CustomerId}</a>|Currency={contactComponent.Currency}</div><div>Email={contactComponent.Email}|IpAddress={contactComponent.IpAddress}|IsRegistered={contactComponent.IsRegistered}!Language={contactComponent.Language}</div>";
                    }

                    if (messageEntity.HasComponent<ItemAvailabilityComponent>())
                    {
                        var itemAvailabilityComponent = messageEntity.GetComponent<ItemAvailabilityComponent>();
                        messageProperty.RawValue = messageProperty.RawValue + $"<div>AvailQty={itemAvailabilityComponent.AvailableQuantity}|AvailDate={itemAvailabilityComponent.AvailableDate}|ItemId=<a href='https://localhost:4200/entityView/Master/{itemAvailabilityComponent.ItemId}'>{itemAvailabilityComponent.ItemId}</a></div>";
                    }

                    var orderSummary = messageEntity.Models.OfType<OrderSummaryModel>().FirstOrDefault();
                    if (orderSummary != null)
                    {
                        messageProperty.RawValue = messageProperty.RawValue + $"<div>Confirm=<a href='https://localhost:4200/entityView/Master/{orderSummary.OrderId}' target='_blank'> {orderSummary.ConfirmationId}</a>|OrderId={orderSummary.OrderId}|Status={orderSummary.Status}</div>";
                    }

                    var totals = messageEntity.Models.OfType<Totals>().FirstOrDefault();
                    if (totals != null)
                    {
                        messageProperty.RawValue = messageProperty.RawValue + $"<div>SubTotal={totals.SubTotal.AsCurrency()}|Adjustments={totals.AdjustmentsTotal.AsCurrency()}|GrandTotal={totals.GrandTotal.AsCurrency()}|Payments={totals.PaymentsTotal.AsCurrency()}|</div>";
                    }
                }
            }

            return Task.FromResult(entityView);
        }
    }
}
