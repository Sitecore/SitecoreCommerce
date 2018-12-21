namespace Plugin.Sample.Catalog.Generator
{
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    using Plugin.Sample.Catalog.Generator.EntityViews;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<IGetEntityViewPipeline>(d =>
                {
                    d.Add<FormGenerateSampleCatalog>().Before<IFormatEntityViewPipeline>();
                })
                .ConfigurePipeline<IFormatEntityViewPipeline>(d =>
                {
                    d.Add<EnsureActions>().After<PopulateEntityViewActionsBlock>();
                })
                .ConfigurePipeline<IDoActionPipeline>(d =>
                {
                    d.Add<DoActionGenerateSampleCatalog>().After<ValidateEntityVersionBlock>();
                })
                .ConfigurePipeline<IConfigureServiceApiPipeline>(d =>
                    d.Add<ConfigureServiceApiBlock>()));

            services.RegisterAllCommands(assembly);
        }
    }
}