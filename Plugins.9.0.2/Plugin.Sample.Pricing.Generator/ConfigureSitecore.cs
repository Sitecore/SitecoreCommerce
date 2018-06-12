
namespace Plugin.Sample.Pricing.Generator
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Promotions;
    using Sitecore.Commerce.Plugin.Coupons;
    using Sitecore.Commerce.Plugin.Pricing;

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
                 d.Add<FormGenerateSamplePriceBook>().Before<IFormatEntityViewPipeline>()
                  .Add<EnsurePriceBookGenerateStats>().Before<IFormatEntityViewPipeline>();
             })
             .ConfigurePipeline<IFormatEntityViewPipeline>(d =>
             {
                 d.Add<EnsureActions>().After<PopulateEntityViewActionsBlock>();
             })
             .ConfigurePipeline<IDoActionPipeline>(
                c =>
                {
                    c.Add<DoActionGenerateSamplePriceBook>().After<ValidateEntityVersionBlock>()
                     .Add<DoActionDeletePriceBook>().After<ValidateEntityVersionBlock>()
                     .Add<DoActionClearPriceCards>().After<ValidateEntityVersionBlock>();
                })

              .ConfigurePipeline<IFormatEntityViewPipeline>(d =>
              {
                  d.Add<EnsureActions>().After<PopulateEntityViewActionsBlock>();
              })
              //Custom End


            services.RegisterAllCommands(assembly);
        }
    }
}