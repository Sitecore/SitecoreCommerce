
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Sample.Entitlement.Enhancements
{
    using Plugin.Sample.Entitlement.Enhancements.EntityViews;

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
                    d.Add<EntityViewCustomerDigitalItems>().Before<IFormatEntityViewPipeline>();
                }));
            
            services.RegisterAllCommands(assembly);
        }
    }
}