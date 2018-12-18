
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Sample.Messaging
{
    using global::Plugin.Sample.Messaging.EntityViews;

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
             .ConfigurePipeline<IGetEntityViewPipeline>(d =>
                 {
                     d.Add<Dashboard>().Before<IFormatEntityViewPipeline>()
                      .Add<FomAddDashboardEntity>().Before<IFormatEntityViewPipeline>();
                 })
             .ConfigurePipeline<IFormatEntityViewPipeline>(d =>
                 {
                     d.Add<EnsureActions>().After<PopulateEntityViewActionsBlock>()
                      .Add<EnsurePluginActions>().After<PopulateEntityViewActionsBlock>();
                 })
              .ConfigurePipeline<IDoActionPipeline>(
                c =>
                {
                    c.Add<DoActionAddDashboardEntity>().After<ValidateEntityVersionBlock>()
                     .Add<DoActionClearMessages>().After<ValidateEntityVersionBlock>();
                })
            );
            
            services.RegisterAllCommands(assembly);
        }
    }
}