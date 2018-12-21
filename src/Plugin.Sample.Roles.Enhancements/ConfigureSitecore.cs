
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Sample.Roles.Enhancements
{
    using global::Plugin.Sample.Roles.Enhancements.EntityViews;

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
             
             .ConfigurePipeline<IGetEntityViewPipeline>(d =>
                 {
                     d.Add<ViewEnsureRoles>().Before<IFormatEntityViewPipeline>();
                 })
             .ConfigurePipeline<IFormatEntityViewPipeline>(d =>
                 {
                     d.Add<EnsurePluginActions>().After<PopulateEntityViewActionsBlock>();
                 }));

            services.RegisterAllCommands(assembly);
        }
    }
}