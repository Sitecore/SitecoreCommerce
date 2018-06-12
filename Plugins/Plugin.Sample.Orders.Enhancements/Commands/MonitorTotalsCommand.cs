// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MonitorTotalsCommand.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   Defines the MonitorTotalsCommand command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Orders.Enhancements
{
    using System;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    //using Sitecore.Commerce.Plugin.Management;
    using Microsoft.Extensions.Logging;
    //using Sitecore.Services.Core.Model;
    using Sitecore.Commerce.EntityViews;
    using System.Linq;
    using Sitecore.Commerce.Plugin.Orders;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the ChildViewResources command.
    /// </summary>
    public class MonitorTotalsCommand : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorTotalsCommand"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public MonitorTotalsCommand(IServiceProvider serviceProvider,
            CommerceCommander commerceCommander) : base(serviceProvider)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// The process of the command
        /// </summary>
        /// <param name="commerceContext">
        /// The commerce context
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> Process(CommerceContext commerceContext)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {

                //var nodeContext = this._commerceCommander.CurrentNodeContext(commerceContext);
                //var runningCommands = nodeContext.GetModel<RunningCommands>();

                //runningCommands.RunningActions.Add(this);

                var pluginPolicy = commerceContext.GetPolicy<PluginPolicy>();

                var customerTotals = new Dictionary<string, OrdersTotals>();

                try
                {
                    var orderTotals = await this._commerceCommander.GetEntity<OrdersTotals>(commerceContext, "Orders_Totals", true);

                    if (!orderTotals.IsPersisted)
                    {
                        orderTotals.Id = "Orders_Totals";
                    }
                    orderTotals.LastRunStarted = DateTimeOffset.UtcNow;
                    orderTotals.LastRunEnded = DateTimeOffset.MinValue;
                    orderTotals.MonitorCycle++;

                    var batchSize = 300;


                    var arg = new FindEntitiesInListArgument(typeof(CommerceEntity), "Orders", System.Convert.ToInt32(orderTotals.LastSkip), batchSize);
                    var ordersBatch = await this._commerceCommander.Pipeline<FindEntitiesInListPipeline>().Run(arg, commerceContext.GetPipelineContextOptions());

                    while (ordersBatch.List.Items.Count > 0)
                    {
                        var returnedOrders = ordersBatch.List.Items.OfType<Order>();

                        foreach (var order in returnedOrders)
                        {
                            

                            orderTotals.Totals.GrandTotal.Amount = orderTotals.Totals.GrandTotal.Amount + order.Totals.GrandTotal.Amount;
                            orderTotals.Totals.SubTotal.Amount = orderTotals.Totals.SubTotal.Amount + order.Totals.SubTotal.Amount;
                            orderTotals.Totals.PaymentsTotal.Amount = orderTotals.Totals.PaymentsTotal.Amount + order.Totals.PaymentsTotal.Amount;
                            orderTotals.Totals.AdjustmentsTotal.Amount = orderTotals.Totals.AdjustmentsTotal.Amount + order.Totals.AdjustmentsTotal.Amount;

                            foreach(var adjustment in order.Adjustments)
                            {
                                var adjustmentType = orderTotals.Adjustments.FirstOrDefault(p => p.Name == adjustment.AdjustmentType);
                                if (adjustmentType == null)
                                {
                                    adjustmentType = new TotalsAdjustmentsModel();
                                    adjustmentType.Name = adjustment.AdjustmentType;
                                    orderTotals.Adjustments.Add(adjustmentType);
                                }
                                adjustmentType.Adjustment.Amount = adjustmentType.Adjustment.Amount + adjustment.Adjustment.Amount;
                            }

                            if (order.HasComponent<ContactComponent>())
                            {
                                OrdersTotals orderTotalsByCust = null;

                                var orderContactComponent = order.GetComponent<ContactComponent>();
                                //var customer = await this._commerceCommander.Command<GetCustomerCommand>();
                                var customerTotalsId = $"Order_Totals_ByCust_{orderContactComponent.CustomerId.Replace("Entity-Customer-", "")}";

                                if (customerTotals.ContainsKey(customerTotalsId))
                                {
                                    orderTotalsByCust = customerTotals[customerTotalsId];
                                }
                                else
                                {
                                    orderTotalsByCust = await this._commerceCommander
                                    .GetEntity<OrdersTotals>(commerceContext, customerTotalsId, true);
                                }

                                

                                if (!orderTotalsByCust.IsPersisted)
                                {
                                    orderTotalsByCust.Id = customerTotalsId;
                                }
                                else
                                {
                                    if (orderTotalsByCust.OrderCount == 0 || orderTotalsByCust.MonitorCycle != orderTotals.MonitorCycle)
                                    {
                                        orderTotalsByCust.Totals.GrandTotal.Amount = 0;
                                        orderTotalsByCust.Totals.SubTotal.Amount = 0;
                                        orderTotalsByCust.Totals.PaymentsTotal.Amount = 0;
                                        orderTotalsByCust.Totals.AdjustmentsTotal.Amount = 0;
                                        orderTotalsByCust.MonitorCycle = orderTotals.MonitorCycle;
                                        orderTotalsByCust.Adjustments.Clear();
                                    }
                                }
                                orderTotalsByCust.OrderCount++;

                                orderTotalsByCust.LastSkip = orderTotalsByCust.LastSkip + batchSize;

                                orderTotalsByCust.Totals.GrandTotal.Amount = orderTotalsByCust.Totals.GrandTotal.Amount + order.Totals.GrandTotal.Amount;
                                orderTotalsByCust.Totals.SubTotal.Amount = orderTotalsByCust.Totals.SubTotal.Amount + order.Totals.SubTotal.Amount;
                                orderTotalsByCust.Totals.PaymentsTotal.Amount = orderTotalsByCust.Totals.PaymentsTotal.Amount + order.Totals.PaymentsTotal.Amount;
                                orderTotalsByCust.Totals.AdjustmentsTotal.Amount = orderTotalsByCust.Totals.AdjustmentsTotal.Amount + order.Totals.AdjustmentsTotal.Amount;

                                foreach (var adjustment in order.Adjustments)
                                {
                                    var adjustmentType = orderTotalsByCust.Adjustments.FirstOrDefault(p => p.Name == adjustment.AdjustmentType);
                                    if (adjustmentType == null)
                                    {
                                        adjustmentType = new TotalsAdjustmentsModel();
                                        adjustmentType.Name = adjustment.AdjustmentType;
                                        orderTotalsByCust.Adjustments.Add(adjustmentType);
                                    }
                                    adjustmentType.Adjustment.Amount = adjustmentType.Adjustment.Amount + adjustment.Adjustment.Amount;
                                }

                                //var ordersTotalByCustPersistResult = await this._commerceCommander.PersistEntity(commerceContext, orderTotalsByCust);

                            }

                            orderTotals.OrderCount++;

                        }
                        orderTotals.LastSkip = orderTotals.LastSkip + batchSize;
                        

                        orderTotals.History.Add(new Messaging.HistoryEntryModel { Name = "MonitorTotals.BatchCompleted", EventMessage = $"Completed batch: {orderTotals.LastSkip}" });

                        orderTotals.LastRunEnded = DateTimeOffset.UtcNow;

                        orderTotals.LastSkip = (orderTotals.LastSkip - batchSize) + returnedOrders.Count();

                        var ordersTotalPersistResult = await this._commerceCommander.PersistEntity(commerceContext, orderTotals);

                        foreach(var orderCustomerTotal in customerTotals)
                        {
                            var ordersTotalByCustPersistResult = await this._commerceCommander.PersistEntity(commerceContext, orderCustomerTotal.Value);
                        }

                        customerTotals.Clear();

                        await Task.Delay(100);
                        arg = new FindEntitiesInListArgument(typeof(CommerceEntity), "Orders", System.Convert.ToInt32(orderTotals.LastSkip), batchSize);
                        ordersBatch = await this._commerceCommander.Pipeline<FindEntitiesInListPipeline>().Run(arg, commerceContext.GetPipelineContextOptions());
                    }

                    //runningCommands.RunningActions.Remove(this);
                }
                catch (Exception ex)
                {
                    commerceContext.Logger.LogError($"ChildViewEnvironments.Exception: Message={ex.Message}");
                    //runningCommands.RunningActions.Remove(this);
                }
                return true;
            }
        }


        /// <summary>
        /// The process of the command
        /// </summary>
        /// <param name="commerceContext">
        /// The commerce context
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<bool> ClearTotals(CommerceContext commerceContext)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                var pluginPolicy = commerceContext.GetPolicy<PluginPolicy>();

                try
                {
                    var orderTotals = await this._commerceCommander.GetEntity<OrdersTotals>(commerceContext, "Orders_Totals", true);

                    if (!orderTotals.IsPersisted)
                    {
                        orderTotals.Id = "Orders_Totals";

                    }

                    orderTotals.Totals.GrandTotal.Amount = 0;
                    orderTotals.Totals.SubTotal.Amount = 0;
                    orderTotals.Totals.PaymentsTotal.Amount = 0;
                    orderTotals.Totals.AdjustmentsTotal.Amount = 0;
                    orderTotals.Adjustments.Clear();

                    orderTotals.LastSkip = 0;
                    var ordersTotalPersistResult = await this._commerceCommander.PersistEntity(commerceContext, orderTotals);


                }
                catch (Exception ex)
                {
                    commerceContext.Logger.LogError($"ChildViewEnvironments.Exception: Message={ex.Message}");
                }
                return true;
            }
        }

    }
}