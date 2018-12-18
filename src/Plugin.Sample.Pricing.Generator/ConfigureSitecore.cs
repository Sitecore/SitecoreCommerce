
namespace Plugin.Sample.Pricing.Generator
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    using Plugin.Sample.Pricing.Generator.EntityViews;

    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using Sitecore.Commerce.EntityViews;

    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config
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
                }));
            
            services.RegisterAllCommands(assembly);
        }
    }
}