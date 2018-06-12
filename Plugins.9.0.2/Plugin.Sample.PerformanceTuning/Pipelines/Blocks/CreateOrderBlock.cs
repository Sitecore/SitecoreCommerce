// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CreateOrderBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.PerformanceTuning
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.ApplicationInsights;
    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using PCat = Sitecore.Commerce.Core.CoreConstants.KnownPerformanceCounters.CategoryNames;
    using PCounter = Sitecore.Commerce.Core.CoreConstants.KnownPerformanceCounters.CounterNames;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Commerce.Core.Commands;

    /// <summary>
    /// Defines a block which creates an order.
    /// </summary>
    [PipelineDisplayName(OrdersConstants.Pipelines.Blocks.CreateOrderBlock)]
    public class CreateOrderBlock : PipelineBlock<CartEmailArgument, Order, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// The delete pipeline.
        /// </summary>
        private readonly IDeleteEntityPipeline deletePipeline;

        /// <summary>
        /// The telemetry client.
        /// </summary>
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// The performance counter command.
        /// </summary>
        private readonly PerformanceCounterCommand performanceCounterCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateOrderBlock"/> class.
        /// </summary>
        /// <param name="deleteEntityPipeline">
        /// The find entity pipeline.
        /// </param>
        /// <param name="telemetryClient">
        /// The Singleton TelemetryClient.
        /// </param>
        /// <param name="performanceCounterCommand">
        /// The Performance Counter Command.
        /// </param>
        public CreateOrderBlock(
            IDeleteEntityPipeline deleteEntityPipeline, 
            TelemetryClient telemetryClient,
            PerformanceCounterCommand performanceCounterCommand,
            CommerceCommander commander)
        {
            this.deletePipeline = deleteEntityPipeline;
            this.telemetryClient = telemetryClient;
            this.performanceCounterCommand = performanceCounterCommand;
            this._commerceCommander = commander;
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
        /// The <see cref="Order"/>.
        /// </returns>
        public override async Task<Order> Run(CartEmailArgument arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: arg can not be null");
            Condition.Requires(arg.Cart).IsNotNull($"{this.Name}: The cart can not be null");

            if (string.IsNullOrEmpty(arg.Email))
            {
                context.Abort(
                    await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Error,
                        "EmailIsRequired",
                        new object[] { null },
                        "Can not create order, email address is required."),
                    context);
                return null;
            }

            var cart = arg.Cart;
            if (!cart.Lines.Any())
            {
                context.Abort(
                    await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Error, 
                        "OrderHasNoLines", 
                        new object[] { cart.Id }, 
                        $"Can not create order, cart {cart.Id} has no lines"), 
                    context);
                return null;
            }
      
            context.CommerceContext.AddModel(cart.Totals);

            if (decimal.Compare(cart.Totals.PaymentsTotal.Amount, cart.Totals.GrandTotal.Amount) != 0)
            {
                context.Abort(
                    await context.CommerceContext.AddMessage(
                        context.GetPolicy<KnownResultCodes>().Error,
                        "InsufficientPayment",
                        null,
                        "Order Payments Total must equal the GrandTotal"), 
                    context);
                return null;
            }

            var contactComponent = cart.GetComponent<ContactComponent>();
            contactComponent.Email = arg.Email;

            var knownOrderListsPolicy = context.GetPolicy<KnownOrderListsPolicy>();
            var globalOrderPolicy = context.GetPolicy<GlobalOrderPolicy>();

            var orderId = $"{CommerceEntity.IdPrefix<Order>()}{Guid.NewGuid():N}";

            var order = new Order
            {
                Id = orderId,
                Name = cart.Name,
                DisplayName = cart.DisplayName,
                Totals = cart.Totals,
                Lines = cart.Lines,
                Components = cart.Components,
                Policies = cart.Policies,
                Adjustments = cart.Adjustments,
                ShopName = cart.ShopName,
                FriendlyId = orderId,
                OrderConfirmationId = string.Empty,
                OrderPlacedDate = DateTimeOffset.UtcNow,
                Status = globalOrderPolicy.CreatedOrderStatus               
            };

            var registerListName = contactComponent.IsRegistered ? knownOrderListsPolicy.AuthenticatedOrders : knownOrderListsPolicy.AnonymousOrders;

            var memberships = new ListMembershipsComponent { Memberships = new List<string> { CommerceEntity.ListName<Order>(), registerListName } };

            if (contactComponent.IsRegistered && !string.IsNullOrEmpty(contactComponent.CustomerId))
            {
                memberships.Memberships.Add(string.Format(context.GetPolicy<KnownOrderListsPolicy>().CustomerOrders, contactComponent.CustomerId));
            }

            order.SetComponent(memberships);

            order.SetComponent(new TransientListMembershipsComponent
            {
                Memberships = new List<string>
                    {
                        knownOrderListsPolicy.PendingOrders
                    }
            });

            // DELETING THE CART 
            //var deleteArg = new DeleteEntityArgument(cart.Id);
            //await this.deletePipeline.Run(deleteArg, context);

            //cart.Lines.Clear();
            //cart.Components.Clear();
            //cart.Policies.Clear();
            //cart.Adjustments.Clear();
            //cart.ItemCount = 0;

            //var persistCartResult = await this._commerceCommander
            //    .PersistEntity(context.CommerceContext, cart);

            context.Logger.LogInformation($"Orders.CreateOrder: OrderId={orderId}|GrandTotal={order.Totals.GrandTotal.CurrencyCode} {order.Totals.GrandTotal.Amount}");

            context.CommerceContext.AddModel(new CreatedOrder { OrderId = orderId });

            var telemetryData = new Dictionary<string, double> { { "GrandTotal", System.Convert.ToDouble(order.Totals.GrandTotal.Amount) } };
            this.telemetryClient.TrackEvent("OrderCreated",null ,telemetryData);

            var grandTotal = Convert.ToInt32(System.Math.Round(order.Totals.GrandTotal.Amount, 0));

            var perfPolicy = context.GetPolicy<PerformancePolicy>();

            if (perfPolicy.WriteCounters)
            {
                await this.performanceCounterCommand.IncrementBy(PCat.SitecoreCommerceMetrics, PCounter.MetricCount, $"Orders.GrandTotal.{order.Totals.GrandTotal.CurrencyCode}", grandTotal, context.CommerceContext);
                await this.performanceCounterCommand.Increment(PCat.SitecoreCommerceMetrics, PCounter.MetricCount, "Orders.Count", context.CommerceContext);
            }

            return order;
        }
    }
}
