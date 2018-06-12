
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Sample.Orders.Enhancements
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using Sitecore.Commerce.Plugin.BusinessUsers;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Fulfillment;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Payments;
    using Sitecore.Commerce.Plugin.Promotions;
    using Sitecore.Commerce.Plugin.Coupons;

    /// <summary>
    /// The configure sitecore class.  This allows a Plugin to wire up new Pipelines or to change existing ones.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services constructor.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            //Detects and registers any Pipeline blocks in the Plugi
            services.RegisterAllPipelineBlocks(assembly);

            //Manipulation of pipelines during startup
            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<IBizFxNavigationPipeline>(d =>
                    {
                        //d.Add<EnsureNavigationView>();
                    })
                .ConfigurePipeline<IGetEntityViewPipeline>(d =>
                    {
                        d.Add<ViewOrders>().Before<IFormatEntityViewPipeline>()
                         .Add<ViewMessagesDashboard>().Before<IFormatEntityViewPipeline>()
                         .Add<ViewOrderMoveLinesUp>().Before<IFormatEntityViewPipeline>();
                    })
                .ConfigurePipeline<IFormatEntityViewPipeline>(d =>
                    {
                        d.Add<EnsureActionOrders>().After<PopulateEntityViewActionsBlock>();
                    })
                .ConfigurePipeline<IDoActionPipeline>(
                    c =>
                    {
                        c.Add<DoActionMonitorTotals>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionClearTotals>().After<ValidateEntityVersionBlock>();
                    })
                .ConfigurePipeline<ICreateOrderPipeline>(d =>
                    {
                        d.Add<FailedOrderMessageBlock>().Before<CreateOrderBlock>()
                         //.Add<SuccessOrderMessageBlock>()
                         ;
                    })
                .ConfigurePipeline<IAddCartLinePipeline>(d =>
                    {
                        d.Add<FailUnavailableItemAdded>();
                    })





                ////Performance Improvement tests
                //.ConfigurePipeline<IGetCartPipeline>(d =>
                //    {
                //        d.Remove<ICalculateCartLinesPipeline>()
                //         .Remove<ICalculateCartPipeline>();
                //    })
                //.ConfigurePipeline<ISetCartFulfillmentPipeline>(d =>
                //{
                //    d.Remove<ValidateCartFulfillmentBlock>()
                //     .Remove<ValidateCartFulfillmentPartyBlock>();
                //})
                //.ConfigurePipeline<ISetCartLinesFulfillmentPipeline>(d =>
                //{
                //    d.Remove<ValidateCartLinesFulfillmentBlock>()
                //     .Remove<ValidateCartLinesFulfillmentPartiesBlock>();
                //})
                //.ConfigurePipeline<IPersistEntityPipeline>(d =>
                //{
                //    d.Remove<IValidateEntityPipeline>()
                //     .Remove<LocalizeEntityBlock>();
                //})
                //.ConfigurePipeline<IAddCartLinePipeline>(d =>
                //{
                //    d.Remove<ValidateSellableItemBlock>();
                //})
                //.ConfigurePipeline<IAddPaymentsPipeline>(d =>
                //{
                //    d
                //    //.Remove<ValidateCartAndPaymentsBlock>()
                //     //.Remove<ValidateCartHasFulfillmentBlock>()
                //     .Remove<ValidateFederatedPaymentBlock>()
                //     ;
                //})
                //.ConfigurePipeline<ICalculateCartLinesPipeline>(d =>
                //{
                //    d
                //     .Remove<ValidateCartLinesPriceBlock>()
                //     .Remove<ValidateCartCouponsBlock>()
                //     .Remove<CalculateCartLinesPromotionsBlock>()
                //     ;
                //})
                //.ConfigurePipeline<IGetCatalogPipeline>(d =>
                //{
                //    d
                //     .Remove<GetCustomRelationshipsBlock>()
                //     ;
                //})
                
            );

            //Detects and registers all commands in the Plugin
            services.RegisterAllCommands(assembly);
        }
    }
}