
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Sample.Plugin.Enhancements
{
    using global::Plugin.Sample.Plugin.Enhancements.EntityViews;

    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using Sitecore.Commerce.Plugin.BusinessUsers;
    using Sitecore.Commerce.EntityViews;
    
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            services.RegisterAllPipelineBlocks(assembly);
            
            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<IBizFxNavigationPipeline>(d =>
                {
                    d.Add<EnsureNavigationView>();
                })
                .ConfigurePipeline<IDoActionPipeline>(
                    c =>
                    {
                        c.Add<DoActionEnablePlugin>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionDisablePlugin>().After<ValidateEntityVersionBlock>();
                    }));

            services.RegisterAllCommands(assembly);
        }
    }
}