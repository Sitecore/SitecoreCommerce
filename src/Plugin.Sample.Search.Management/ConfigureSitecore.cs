
namespace Plugin.Sample.Search.Management
{
    using System.Reflection;

    using global::Plugin.Sample.Search.Management.EntityViews;

    using Microsoft.Extensions.DependencyInjection;
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
                }));

            services.RegisterAllCommands(assembly);
        }
    }
}