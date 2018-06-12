// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClearCartBlock.cs" company="Sitecore Corporation">
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
    using Sitecore.Commerce.Plugin.Carts;

    /// <summary>
    /// Defines a block which creates an order.
    /// </summary>
    [PipelineDisplayName("ClearCartBlock")]
    public class ClearCartBlock : PipelineBlock<Order, Order, CommercePipelineExecutionContext>
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
        public ClearCartBlock(
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
        public override async Task<Order> Run(Order order, CommercePipelineExecutionContext context)
        {
            Condition.Requires(order).IsNotNull($"{this.Name}: order can not be null");

            
            var cart = context.CommerceContext.GetObject<Cart>();
            if (cart == null)
            {
                return order;
            }

            cart.Lines.Clear();
            cart.Components.Clear();
            cart.Policies.Clear();
            cart.Adjustments.Clear();
            cart.ItemCount = 0;

            var persistCartResult = await this._commerceCommander
                .PersistEntity(context.CommerceContext, cart);

            return order;
        }
    }
}
