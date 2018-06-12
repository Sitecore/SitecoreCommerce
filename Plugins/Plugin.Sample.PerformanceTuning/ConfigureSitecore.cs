
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Sample.PerformanceTuning
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using Sitecore.Commerce.Plugin.BusinessUsers;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Inventory;
    using Sitecore.Commerce.Plugin.Availability;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Fulfillment;
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
                //.ConfigurePipeline<IBizFxNavigationPipeline>(d =>
                //    {
                //        //d.Add<EnsureNavigationView>();
                //    })
                //.ConfigurePipeline<IGetEntityViewPipeline>(d =>
                //    {
                //        //d.Add<Dashboard>().Before<IFormatEntityViewPipeline>()
                //        // .Add<FomAddDashboardEntity>().Before<IFormatEntityViewPipeline>();
                //    })
                //.ConfigurePipeline<IFormatEntityViewPipeline>(d =>
                //    {
                //        //d.Add<EnsureActions>().After<PopulateEntityViewActionsBlock>();
                //    })
                // .ConfigurePipeline<IDoActionPipeline>(
                //   c =>
                //   {
                //       //c.Add<DoActionAddDashboardEntity>().After<ValidateEntityVersionBlock>()
                //       // .Add<DoActionRemoveDashboardEntity>().After<ValidateEntityVersionBlock>();
                //   })

                // ############# TEST Performance Tuning Changes TEST ###################

                //.ConfigurePipeline<ICalculateSellableItemPricesPipeline>(d =>
                //{
                //    d
                //     .Remove<CalculateSellableItemSellPriceBlock>()
                //     ;
                //})


                //Performance Improvement tests
                //.ConfigurePipeline<IGetCartPipeline>(d =>
                //{
                //    d.Remove<ICalculateCartLinesPipeline>()
                //     .Remove<ICalculateCartPipeline>();
                //})
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
                //     //.Remove<ValidateCartAndPaymentsBlock>()
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



                //############### Performance Tuning

                .ConfigurePipeline<IGetSellableItemPipeline>(d =>
                {
                    d
                     //.Remove<EnsureSellableItemAvailabilityPoliciesBlock>()
                     //.Add<EnsureSellableItemAvailabilityPoliciesBlock>().After<GetSellableItemBlock>()
                     .Replace<Sitecore.Commerce.Plugin.Inventory.ResolveSellableItemInventoryInformationBlock, ResolveSellableItemInventoryInformationBlock>()
                     ;
                })

            //.ConfigurePipeline<IGetCatalogPipeline>(d =>
            //{
            //    d
            //     .Remove<GetCustomRelationshipsBlock>()
            //     ;
            //})
            //############### Performance Tuning


            // ############# TEST End Performance Tuning Changes TEST ###################

            );

            //Detects and registers all commands in the Plugin
            services.RegisterAllCommands(assembly);
        }
    }
}