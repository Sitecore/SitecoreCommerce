
namespace Plugin.Sample.Promotions.Generator
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Plugin.Sample.Promotions.Generator.EntityViews;
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
                }));

            services.RegisterAllCommands(assembly);
        }
    }
}