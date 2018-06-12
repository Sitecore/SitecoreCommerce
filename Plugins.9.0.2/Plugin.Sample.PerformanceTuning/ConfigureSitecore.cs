
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Sample.PerformanceTuning
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Catalog;
    using Sitecore.Commerce.Plugin.Inventory;
    using Sitecore.Commerce.Plugin.Carts;
    using Sitecore.Commerce.Plugin.Fulfillment;
    using Sitecore.Commerce.Plugin.Payments;
    using Sitecore.Commerce.Plugin.Promotions;
    using Sitecore.Commerce.Plugin.Coupons;
    using Sitecore.Commerce.Plugin.Pricing;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Commerce.Plugin.Customers;

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

                //Fix - Null reference error causing whole pipeline to fail
                .ConfigurePipeline<IGetEntityViewPipeline>(d =>
                    {
                        d.Replace<Sitecore.Commerce.Plugin.Orders.GetOrderSummaryEntityViewBlock, GetOrderSummaryEntityViewBlock>();
                    })

                // We need to stop the Minions from starting up since we are calling the Minions manually
                //This is only to support the Order Loader from the Console tool and not needed in the release
                .ConfigurePipeline<IStartEnvironmentPipeline>(d =>
                {
                    d.Remove<StartEnvironmentMinionsBlock>();
                })

                // ############# TEST Performance Tuning Changes TEST ###################

                //FIX - Filter snapshots by tags to remove cloning and other performance issues
                .ConfigurePipeline<IResolveActivePriceSnapshotByTagsPipeline>(d =>
                //Removing cloning of price cards
                d
                .Replace<Sitecore.Commerce.Plugin.Pricing.FilterPriceSnapshotsByTagsBlock, FilterPriceSnapshotsByTagsBlock>()
                )

                //Remove the calculation of the Cart when GetCart is called.
                //To productize, we would not remove it but would evaluate it to see how long it has been 
                //since it was last calculated and only calculate again if it has been more than 1 minute (configured by policy)
                .ConfigurePipeline<IGetCartPipeline>(d =>
                {
                    d.Remove<ICalculateCartLinesPipeline>()
                     .Remove<ICalculateCartPipeline>();
                })

                //Test to see how much calculating promotions is costing (+2 min)
                //Make it easier to enable and disable promotions
                .ConfigurePipeline<ICalculateCartPipeline>(d =>
                {
                    d.Remove<CalculateCartPromotionsBlock>();
                })
                .ConfigurePipeline<ICalculateCartLinesPipeline>(d =>
                {
                    d.Remove<CalculateCartLinesPromotionsBlock>();
                })

                .ConfigurePipeline<ICalculateCartLinesPipeline>(d =>
                {
                    //Skips populating line item product information if it is already populated.
                    //It determines this by checking to see if the Line Item has policies
                    d.Replace<Sitecore.Commerce.Plugin.Carts.PopulateCartLineItemsBlock, PopulateCartLineItemsBlock>()
                    //Doesn't calculate pricing for an item if it sees that there is already a PurchaseOptionMoneyPolicy.
                    //To productize, it should only calculate pricing if it has been more than one minute since the last time it calculated pricing (configurable by policy)
                     .Replace<Sitecore.Commerce.Plugin.Catalog.CalculateCartLinesPriceBlock, CalculateCartLinesPriceBlock>()
                     //Validate coupons loads each coupon and validates the allocation every time the cart is calculated.
                     //This should only happen once (possible twice if we want a final check when the order is placed...)
                     .Remove<ValidateCartCouponsBlock>();
                })

                //TEST - remove validations to see how much they are costing (+1min)

                //GetFulfillmentMethodsBlock should have caching to avoid database calls for list that doesn't change
                .ConfigurePipeline<ISetCartFulfillmentPipeline>(d =>
                {
                    d
                    .Remove<ValidateCartFulfillmentBlock>()
                     .Remove<ValidateCartFulfillmentPartyBlock>();
                })
                .ConfigurePipeline<ISetCartLinesFulfillmentPipeline>(d =>
                {
                    d
                    .Remove<ValidateCartLinesFulfillmentBlock>()
                     .Remove<ValidateCartLinesFulfillmentPartiesBlock>();
                })

                .ConfigurePipeline<IPersistEntityPipeline>(d =>
                {
                    d
                     //Tested but did not appear to significantly impact perf.
                     //.Remove<IValidateEntityPipeline>()
                     //Tested but did not appear to significantly impact perf.
                     //.Remove<LocalizeEntityBlock>()
                     //Tested but did not appear to affect perf
                     //.Remove<IEntityPersistedPipeline>()
                     //This changes the Process to assume the Entity has already been added to the Lists, if it has already been persisted
                     //This reduces the overhead of validating that the Entity is in the List each time it is persisted.
                     //Normally the list memberships don't change, list memberships that can change go in the transient list membership
                     //An alternative, if we don't want to "trust" that the lists have been updated would be to store the last known lists and compare on persist
                     .Replace<Sitecore.Commerce.Plugin.ManagedLists.ProcessListMembershipsBlock, ProcessListMembershipsBlock>();
                })

                //Remove validation to see how much it is costing
                //.ConfigurePipeline<IAddCartLinePipeline>(d =>
                //{
                //    d.Remove<ValidateSellableItemBlock>();
                //})

                .ConfigurePipeline<IAddPaymentsPipeline>(d =>
                {
                    d
                    //This includes a ValidateParty call, which seems more expensive than it should be.
                    //We should separate out the ValidateParty to it's own pipeline block and investigate why it is slow
                     .Remove<ValidateFederatedPaymentBlock>();
                })

                .ConfigurePipeline<IPopulateLineItemPipeline>(d =>
                {
                    //This is removed since we want to only load the SellableItem when we have to.
                    //Once the item is added to the Cart, we should only need the sellableitem to recalculate pricing
                    d.Remove<LoadLineItemSellableItemBlock>()
                     .Replace<Sitecore.Commerce.Plugin.Inventory.PopulateLineItemInventoryBlock, PopulateLineItemInventoryBlock>();
                })

                .ConfigurePipeline<IGetSellableItemPipeline>(d =>
                {
                    d.Replace<Sitecore.Commerce.Plugin.Inventory.ResolveSellableItemInventoryInformationBlock, ResolveSellableItemInventoryInformationBlock>();
                })

                .ConfigurePipeline<IGetInventoryInformationPipeline>(d =>
                {
                    d.Replace<Sitecore.Commerce.Plugin.Inventory.GetInventoryInformationBlock, GetInventoryInformationBlock>();
                })

                .ConfigurePipeline<IFindEntityPipeline>(d =>
                {
                    d.Replace<Sitecore.Commerce.Core.LoadLocalizationEntityBlock, LoadLocalizationEntityBlock>();
                })

                //FIX - Remove Cart delete from CreateOrderBlock and add a ClearCartBlock, which clears the cart but does not delete it
                .ConfigurePipeline<ICreateOrderPipeline>(d =>
                {
                    d.Replace<Sitecore.Commerce.Plugin.Orders.CreateOrderBlock, CreateOrderBlock>()
                     .Add<ClearCartBlock>();;
                })

            // ############# TEST End Performance Tuning Changes TEST ###################

            );

            //Detects and registers all commands in the Plugin
            services.RegisterAllCommands(assembly);
        }
    }
}