
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.Orders.Enhancements
{
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using System.Linq;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Availability;

    /// <summary>
    /// Defines a block which validates, inserts and updates Actions for this Plugin.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("ViewMessagesDashboard")]
    public class ViewMessagesDashboard : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMessagesDashboard"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public ViewMessagesDashboard(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

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
                    var messageEntity = foundEntity.Entity as Messaging.MessageEntity;
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

            //var orderTotals = await this._commerceCommander.GetEntity<OrdersTotals>(context.CommerceContext, "Orders_Totals", true);

            //if (orderTotals.IsPersisted)
            //{
            //    var ordersTotalsView = new EntityView
            //    {
            //        Name = "Orders.Enhancements.Totals",
            //        DisplayName = "Orders Totals",
            //        UiHint = "Flat",
            //        DisplayRank = 100,
            //        Icon = pluginPolicy.Icon,
            //        ItemId = "",
            //    };
            //    entityView.ChildViews.Add(ordersTotalsView);

            //    //orderTotals.Id = "Orders_Totals";
            //    ordersTotalsView.Properties.Add(
            //    new ViewProperty
            //    {
            //        Name = "GrandTotal",
            //        IsHidden = false,
            //        IsReadOnly = true,
            //        RawValue = orderTotals.Totals.GrandTotal.AsCurrency()
            //    });

            //    ordersTotalsView.Properties.Add(
            //    new ViewProperty
            //    {
            //        Name = "Adjustments",
            //        IsHidden = false,
            //        IsReadOnly = true,
            //        RawValue = orderTotals.Totals.AdjustmentsTotal.AsCurrency()
            //    });

            //    ordersTotalsView.Properties.Add(
            //    new ViewProperty
            //    {
            //        Name = "Payments",
            //        IsHidden = false,
            //        IsReadOnly = true,
            //        RawValue = orderTotals.Totals.PaymentsTotal.AsCurrency()
            //    });

            //    ordersTotalsView.Properties.Add(
            //    new ViewProperty
            //    {
            //        Name = "SubTotal",
            //        IsHidden = false,
            //        IsReadOnly = true,
            //        RawValue = orderTotals.Totals.SubTotal.AsCurrency()
            //    });

            //}

            return Task.FromResult(entityView);
        }
    }
}
