
namespace Plugin.Sample.Search.Management
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using Sitecore.Commerce.Plugin.BusinessUsers;
    using Sitecore.Commerce.EntityViews;

    /// <summary>
    /// The carts configure sitecore class.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config

             // Custom Start
             .ConfigurePipeline<IBizFxNavigationPipeline>(d =>
             {
                 d.Add<EnsureNavigationView>();
             })
             .ConfigurePipeline<IDoActionPipeline>(
                c =>
                {
                    c.Add<DoActionRebuildScope>().After<ValidateEntityVersionBlock>()
                     .Add<DoActionCreateSearchIndex>().After<ValidateEntityVersionBlock>()
                     .Add<DoActionDeleteSearchIndex>().After<ValidateEntityVersionBlock>();
                })
             .ConfigurePipeline<IGetEntityViewPipeline>(d =>
             {
                 d.Add<Dashboard>().Before<IFormatEntityViewPipeline>()
                  .Add<FormCreateIndex>().Before<IFormatEntityViewPipeline>()
                 .Add<EntityViewEnsureSearchScopes>().Before<IFormatEntityViewPipeline>();
             })
             .ConfigurePipeline<IFormatEntityViewPipeline>(d =>
             {
                 d.Add<EnsureActions>().After<PopulateEntityViewActionsBlock>()
                  .Add<EnsurePluginActions>().After<PopulateEntityViewActionsBlock>();
             })
            //Custom End


            );

            services.RegisterAllCommands(assembly);
        }
    }
}