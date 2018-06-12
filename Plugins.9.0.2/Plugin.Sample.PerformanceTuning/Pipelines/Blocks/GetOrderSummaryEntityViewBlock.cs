// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetOrderSummaryEntityViewBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.PerformanceTuning
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Commerce.EntityViews;

    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Commerce.Plugin.Orders;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Defines a block which populates an EntityView for an Order with the View named Summary.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName(OrdersConstants.Pipelines.Blocks.GetOrderSummaryEntityView)]
    public class GetOrderSummaryEntityViewBlock : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetOrderSummaryEntityViewBlock" /> class.
        /// </summary>
        /// <param name="commander">The commander.</param>
        public GetOrderSummaryEntityViewBlock(CommerceCommander commander)
        {
            this._commerceCommander = commander;
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="entityView">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="EntityView"/>.
        /// </returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: the argument can not be null.");

            var request = context.CommerceContext.GetObjects<EntityViewArgument>().FirstOrDefault();
            if (request == null)
            {
                // Do nothing if there is no request
                return entityView;
            }

            if (request.ViewName != context.GetPolicy<KnownOrderViewsPolicy>().Summary
                && request.ViewName != context.GetPolicy<KnownOrderViewsPolicy>().Master)
            {
                // Do nothing if this request is for a different view
                return entityView;
            }

            var order = request.Entity as Order;
            if (request.Entity == null || order == null)
            {
                // Do nothing if there is no entity loaded or it isn't an order
                return entityView;
            }

            EntityView entityViewToProcess;

            try
            {
                if (request.ViewName == context.GetPolicy<KnownOrderViewsPolicy>().Master)
                {
                    var childEntityView = new EntityView { EntityId = order.Id, Name = context.GetPolicy<KnownOrderViewsPolicy>().Summary, DisplayRank = 100 };
                    entityView.ChildViews.Add(childEntityView);
                    entityViewToProcess = childEntityView;
                }
                else
                {
                    entityViewToProcess = entityView;
                }

                entityViewToProcess.Properties.Add(new ViewProperty { Name = "OrderConfirmationId", IsReadOnly = true, RawValue = order.OrderConfirmationId });
                entityViewToProcess.Properties.Add(new ViewProperty { Name = "DateCreated", IsReadOnly = true, RawValue = order.OrderPlacedDate, UiType = "FullDateTime" });
                entityViewToProcess.Properties.Add(new ViewProperty { Name = "DateUpdated", IsReadOnly = true, RawValue = order.DateUpdated, UiType = "FullDateTime" });
                entityViewToProcess.Properties.Add(new ViewProperty { Name = "Status", IsReadOnly = true, RawValue = order.Status });
                entityViewToProcess.Properties.Add(new ViewProperty { Name = "ShopName", IsReadOnly = true, RawValue = order.ShopName });

                if (order.HasComponent<ContactComponent>())
                {
                    entityViewToProcess.Properties.Add(new ViewProperty { Name = "CustomerEmail", IsReadOnly = true, RawValue = order.GetComponent<ContactComponent>().Email });
                }

                if (order.HasComponent<OnHoldOrderComponent>())
                {
                    var cart = await this._commerceCommander.Command<GetOnHoldOrderCartCommand>().Process(context.CommerceContext, order);

                    entityViewToProcess.Properties.Add(new ViewProperty { Name = "OrderSubTotal", IsReadOnly = true, RawValue = cart.Totals.SubTotal });
                    entityViewToProcess.Properties.Add(new ViewProperty { Name = "OrderAdjustmentsTotal", IsReadOnly = true, RawValue = cart.Totals.AdjustmentsTotal });
                    entityViewToProcess.Properties.Add(new ViewProperty { Name = "OrderGrandTotal", IsReadOnly = true, RawValue = cart.Totals.GrandTotal });
                    entityViewToProcess.Properties.Add(new ViewProperty { Name = "OrderPaymentsTotal", IsReadOnly = true, RawValue = cart.Totals.PaymentsTotal });
                }
                else
                {
                    entityViewToProcess.Properties.Add(new ViewProperty { Name = "OrderSubTotal", IsReadOnly = true, RawValue = order.Totals.SubTotal });
                    entityViewToProcess.Properties.Add(new ViewProperty { Name = "OrderAdjustmentsTotal", IsReadOnly = true, RawValue = order.Totals.AdjustmentsTotal });
                    entityViewToProcess.Properties.Add(new ViewProperty { Name = "OrderGrandTotal", IsReadOnly = true, RawValue = order.Totals.GrandTotal });
                    entityViewToProcess.Properties.Add(new ViewProperty { Name = "OrderPaymentsTotal", IsReadOnly = true, RawValue = order.Totals.PaymentsTotal });
                }

                await GetViewTitle(entityView, context, order);
            }
            catch(Exception ex)
            {
                context.Logger.LogError(ex, "GetOrderSummaryEntityViewBlock.Error");
            }
            return entityView;
        }

        private async Task GetViewTitle(EntityView entityView, CommercePipelineExecutionContext context, Order order)
        {
            var viewName = await this._commerceCommander
                            .Pipeline<IGetEntityViewLocalizedTermPipeline>()
                            .Run(new ViewLocalizedTermArgument("OrderDetails", "ViewName", null), context);


            var viewTitle = viewName == null ? $"OrderDetails: {order.OrderConfirmationId}" : $"{viewName.Value}: {order.OrderConfirmationId}";

            var displayNameProperty = entityView.Properties.FirstOrDefault(p => p.Name.Equals("DisplayName", StringComparison.OrdinalIgnoreCase));
            if (displayNameProperty != null)
            {
                displayNameProperty.RawValue = viewTitle;
            }
            else
            {
                entityView.Properties.Add(new ViewProperty
                {
                    Name = "DisplayName",
                    RawValue = viewTitle
                });
            }
        }
    }
}
