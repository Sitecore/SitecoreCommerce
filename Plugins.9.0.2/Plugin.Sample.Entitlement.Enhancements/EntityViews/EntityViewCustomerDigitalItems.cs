// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityViewCustomerDigitalItems.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Entitlement.Enhancements
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    //using Sitecore.Commerce.Plugin.DigitalItems;
    using Sitecore.Commerce.Plugin.Customers;
    using Sitecore.Commerce.Plugin.Entitlements;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Commerce.Plugin.Carts;

    /// <summary>
    /// Defines a block which populates an EntityView for a list of Coupons with the View named Public Coupons.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("EntityViewCustomerDigitalItems")]
    public class EntityViewCustomerDigitalItems : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityViewCustomerDigitalItems"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EntityViewCustomerDigitalItems(CommerceCommander commerceCommander)
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

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            if (!(entityViewArgument.Entity is Customer))
            {
                return entityView;
            }

            try
            {
                var customer = entityViewArgument.Entity as Customer;

                var customerEntitlementsView = entityView.ChildViews.FirstOrDefault(p => p.Name == "CustomerEntitlements") as EntityView;
                if (customerEntitlementsView != null)
                {
                    foreach (var entityViewChild in customerEntitlementsView.ChildViews.OfType<EntityView>())
                    {
                        var entitlement = await this._commerceCommander
                                .GetEntity<Entitlement>(context.CommerceContext, entityViewChild.ItemId, false);

                        var order = await this._commerceCommander.GetEntity<Order>(context.CommerceContext, entitlement.Order.EntityTarget, false);
                        var orderEntitlementsComponent = order.GetComponent<EntitlementsComponent>();
                        var entitlementLine = orderEntitlementsComponent.Entitlements.FirstOrDefault(p => p.EntityTarget == entitlement.Id);
                        var orderLine = order.Lines.First(p => p.Id == entitlementLine.ItemTarget);
                        var cartProductComponent = orderLine.GetComponent<CartProductComponent>();

                        var imageId = cartProductComponent.Image.SitecoreId.Replace("-", "").Replace("{", "").Replace("}", "");

                        var properties = new List<ViewProperty>();

                        properties.Add(
                            new ViewProperty
                            {
                                Name = "Image",
                                IsHidden = false,
                                IsRequired = false,
                                RawValue = $"<img alt='This is the alternate' height=50 width=50 src='http://sxa.storefront.com/-/media/{imageId}.ashx'/>",
                                UiType = "Html",
                                OriginalType = "Html"
                            });

                        properties.Add(
                            new ViewProperty
                            {
                                Name = "Name",
                                IsHidden = false,
                                IsRequired = false,
                                RawValue = cartProductComponent.Name
                            });

                        properties.Add(
                            new ViewProperty
                            {
                                Name = "Created",
                                IsHidden = false,
                                IsRequired = false,
                                RawValue = entitlement.DateCreated?.ToString("yyyy-MMM-dd hh:mm")
                            });

                        properties.Add(
                            new ViewProperty
                            {
                                Name = "Order",
                                IsHidden = false,
                                IsRequired = false,
                                RawValue = order.Id
                            });


                        properties.AddRange(entityViewChild.Properties);
                        entityViewChild.Properties = properties;

                    }
                }
            }
            catch(Exception ex)
            {
                context.CommerceContext.LogException($"DigitalItems.Enhancements.EntityViewCustomerDigitalItems.Run.Exception", ex);
            }

            return entityView;
        }

    }

}
