
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.Orders.Enhancements
{
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;

    /// <summary>
    /// Defines a block which validates, inserts and updates Actions for this Plugin.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("ViewOrders")]
    public class ViewOrders : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewOrders"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public ViewOrders(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            OrdersTotals orderTotals = null;

            if (entityView.EntityId.Contains("Entity-Customer-"))
            {
                var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);
                orderTotals = await this._commerceCommander.GetEntity<OrdersTotals>(context.CommerceContext, $"Order_Totals_ByCust_{entityViewArgument.EntityId.Replace("Entity-Customer-", "")}", true);
                if (!orderTotals.IsPersisted)
                {
                    return entityView;
                }
            }

            else if (entityView.Name == "OrdersDashboard")
            {
                orderTotals = await this._commerceCommander.GetEntity<OrdersTotals>(context.CommerceContext, "Orders_Totals", true);

                if (!orderTotals.IsPersisted)
                {
                    return entityView;
                }

            }

            if (orderTotals == null)
            {
                return entityView;
            }

            if (orderTotals.IsPersisted)
            {
                var ordersTotalsView = new EntityView
                {
                    Name = "Orders.Enhancements.Totals",
                    DisplayName = "Order Totals",
                    UiHint = "Flat",
                    DisplayRank = 10,
                    Icon = pluginPolicy.Icon,
                    ItemId = "",
                };
                entityView.ChildViews.Add(ordersTotalsView);

                ordersTotalsView.Properties.Add(
                new ViewProperty
                {
                    Name = "Orders",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = orderTotals.OrderCount
                });

                ordersTotalsView.Properties.Add(
                new ViewProperty
                {
                    Name = "GrandTotal",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = orderTotals.Totals.GrandTotal.AsCurrency()
                });

                //ordersTotalsView.Properties.Add(
                //new ViewProperty
                //{
                //    Name = "Adjustments",
                //    IsHidden = false,
                //    IsReadOnly = true,
                //    RawValue = orderTotals.Totals.AdjustmentsTotal.AsCurrency()
                //});

                ordersTotalsView.Properties.Add(
                new ViewProperty
                {
                    Name = "Payments",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = orderTotals.Totals.PaymentsTotal.AsCurrency()
                });

                ordersTotalsView.Properties.Add(
                new ViewProperty
                {
                    Name = "SubTotal",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = orderTotals.Totals.SubTotal.AsCurrency()
                });
                ordersTotalsView.Properties.Add(
                new ViewProperty
                {
                    Name = "Updated",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = orderTotals.DateUpdated.Value.ToString("yyyy-MMM-dd hh:mm")
                });
                ordersTotalsView.Properties.Add(
                new ViewProperty
                {
                    Name = "Version",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = orderTotals.Version
                });
                ordersTotalsView.Properties.Add(
                new ViewProperty
                {
                    Name = "LastRecord",
                    DisplayName = "Last Record",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = orderTotals.LastSkip
                });

                ordersTotalsView.Properties.Add(
                new ViewProperty
                {
                    Name = "LastRunStart",
                    DisplayName = "Last Run Start",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = orderTotals.LastRunStarted.ToString("yyyy-MMM-dd hh:mm")
                });

                ordersTotalsView.Properties.Add(
                new ViewProperty
                {
                    Name = "LastRunEnd",
                    DisplayName = "Last Run End",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = orderTotals.LastRunEnded.ToString("yyyy-MMM-dd hh:mm")
                });

                ordersTotalsView.Properties.Add(
                new ViewProperty
                {
                    Name = "Run Time",
                    DisplayName = "Run Time (s)",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = (orderTotals.LastRunEnded - orderTotals.LastRunStarted).TotalSeconds.ToString()
                });

                if (orderTotals.Adjustments.Count > 0)
                {
                    foreach(var adjustment in orderTotals.Adjustments)
                    {
                        adjustment.Adjustment.CurrencyCode = "USD";
                        ordersTotalsView.Properties.Add(
                            new ViewProperty
                            {
                                Name = adjustment.Name,
                                IsHidden = false,
                                IsReadOnly = true,
                                RawValue = adjustment.Adjustment
                            });
                    }
                }
            }
            return entityView;
        }
    }
}
