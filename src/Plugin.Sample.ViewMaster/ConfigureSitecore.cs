
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Sample.ViewMaster
{
    using global::Plugin.Sample.ViewMaster.EntityViews;

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
                      .Add<FomAddDashboardEntity>().Before<IFormatEntityViewPipeline>()
                      .Add<ViewMasterSessionRecord>().Before<IFormatEntityViewPipeline>()
                      .Add<CaptureView>();
                 })
             .ConfigurePipeline<IFormatEntityViewPipeline>(d =>
                 {
                     d.Add<EnsureActions>().After<PopulateEntityViewActionsBlock>()
                      .Add<EnsurePluginActions>().After<PopulateEntityViewActionsBlock>();
                 })
              .ConfigurePipeline<IDoActionPipeline>(
                c =>
                {
                    c.Add<DoActionCaptureAction>().After<ValidateEntityVersionBlock>()
                     .Add<DoActionClearAllEvents>().After<ValidateEntityVersionBlock>()
                     .Add<DoActionRemoveDashboardEntity>().After<ValidateEntityVersionBlock>();
                }));
            
            services.RegisterAllCommands(assembly);
        }
    }
}