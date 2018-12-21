namespace Plugin.Sample.Entitlement.Enhancements.EntityViews
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Customers;
    using Sitecore.Commerce.Plugin.Entitlements;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("EntityViewCustomerDigitalItems")]
    public class EntityViewCustomerDigitalItems : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public EntityViewCustomerDigitalItems(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
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
                if (entityView.ChildViews.FirstOrDefault(p => p.Name == "CustomerEntitlements") is EntityView customerEntitlementsView)
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

                        var properties = new List<ViewProperty>
                        {
                            new ViewProperty
                            {
                                Name = "Image",
                                IsHidden = false,
                                IsRequired = false,
                                RawValue =
                                    $"<img alt='This is the alternate' height=50 width=50 src='http://sxa.storefront.com/-/media/{imageId}.ashx'/>",
                                UiType = "Html",
                                OriginalType = "Html"
                            },
                            new ViewProperty
                            {
                                Name = "Name",
                                IsHidden = false,
                                IsRequired = false,
                                RawValue = cartProductComponent.Name
                            },
                            new ViewProperty
                            {
                                Name = "Created",
                                IsHidden = false,
                                IsRequired = false,
                                RawValue = entitlement.DateCreated?.ToString("yyyy-MMM-dd hh:mm")
                            },
                            new ViewProperty
                            {
                                Name = "Order", IsHidden = false, IsRequired = false, RawValue = order.Id
                            }
                        };

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
