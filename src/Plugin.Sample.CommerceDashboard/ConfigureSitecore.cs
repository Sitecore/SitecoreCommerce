
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Sample.CommerceDashboard
{
    using Plugin.Sample.CommerceDashboard.EntityViews;

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
                     d.Add<EnsureActions>().After<PopulateEntityViewActionsBlock>();
                 })
              .ConfigurePipeline<IDoActionPipeline>(
                c =>
                {
                    c.Add<DoActionAddDashboardEntity>().After<ValidateEntityVersionBlock>()
                     .Add<DoActionRemoveDashboardEntity>().After<ValidateEntityVersionBlock>();
                })
            );

            services.RegisterAllCommands(assembly);
        }
    }
}