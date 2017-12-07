
namespace Plugin.Sample.Promotions.Generator
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Promotions;
    using Sitecore.Commerce.Plugin.Coupons;

    /// <summary>
    /// The carts configure sitecore class.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config
             
             // Custom Start
             //.ConfigurePipeline<IBizFxNavigationPipeline>(d =>
             //{
             //    d.Add<EnsureNavigationView>();
             //})
             .ConfigurePipeline<IGetEntityViewPipeline>(d =>
             {
                 d.Add<FormGenerateSamplePromotionBook>().Before<IFormatEntityViewPipeline>();
             })
             .ConfigurePipeline<IFormatEntityViewPipeline>(d =>
             {
                 d.Add<EnsureActions>().After<PopulateEntityViewActionsBlock>();
             })
             .ConfigurePipeline<IDoActionPipeline>(
                c =>
                {
                    c.Add<DoActionGenerateSamplePromotionBook>().After<ValidateEntityVersionBlock>();
                })
            .ConfigurePipeline<IDiscoverPromotionsPipeline>(
                c =>
                {
                    c.Add<EvaluatePromosSearchForPromotions>().After<SearchForPromotionsBlock>()
                     .Add<EvaluatePromosFilterByValidDate>().After<FilterPromotionsByValidDateBlock>()
                     .Add<EvaluatePromosFilterNotApproved>().After<FilterNotApprovedPromotionsBlock>()
                     .Add<EvaluatePromosFilterByItems>().After<FilterPromotionsByItemsBlock>()
                     .Add<EvaluatePromosFilterByAssociatedCatalog>().After<FilterPromotionsByBookAssociatedCatalogsBlock>()
                     .Add<EvaluatePromosFilterByCoupon>().After<FilterPromotionsByCouponBlock>()
                     .Add<EvaluatePromosFilterByBenefitType>().After<FilterPromotionsByBenefitTypeBlock>();
                })

               //Custom End

               .ConfigurePipeline<IConfigureServiceApiPipeline>(configure => configure.Add<ConfigureServiceApiBlock>())
						);

            services.RegisterAllCommands(assembly);
        }
    }
}