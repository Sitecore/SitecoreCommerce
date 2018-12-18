
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Sample.Catalog.Enhancements
{
    using Plugin.Sample.Catalog.Enhancements.EntityViews;

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
             .ConfigurePipeline<IFormatEntityViewPipeline>(d =>
                 {
                     d.Add<EnsureInventoryEditActions>().After<PopulateEntityViewActionsBlock>();
                 }));
            
            services.RegisterAllCommands(assembly);
        }
    }
}