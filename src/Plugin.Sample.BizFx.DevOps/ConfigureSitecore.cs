namespace Plugin.Sample.BizFx.DevOps
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Plugin.Sample.BizFx.DevOps.EntityViews;
    using Plugin.Sample.BizFx.DevOps.Pipelines.Blocks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.BusinessUsers;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

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
                        .Add<EntityViewAppService>().Before<IFormatEntityViewPipeline>()
                        .Add<FormEditPolicy>().Before<IFormatEntityViewPipeline>()
                        .Add<FormPullEnvironment>().Before<IFormatEntityViewPipeline>()
                        .Add<EntityViewEnvironment>().Before<IFormatEntityViewPipeline>();
                })
                .ConfigurePipeline<IFormatEntityViewPipeline>(d =>
                {
                    d.Add<EnsureActions>().After<PopulateEntityViewActionsBlock>();
                })
                .ConfigurePipeline<IDoActionPipeline>(
                    c =>
                    {
                        c.Add<DoActionBootstrapAppService>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionCleanEnvironment>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionCleanCurrentEnvironment>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionInitializeEnvironment>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionInitializeCurrentEnvironment>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionAddAppService>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionRemoveAppService>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionEditPolicy>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionRemoveEnvironment>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionPullEnvironment>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionExportEnvironment>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionClearAppServices>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionInitializeAppServiceSamples>().After<ValidateEntityVersionBlock>();
                    })
                .ConfigurePipeline<IFindEntityPipeline>(d =>
                {
                    d.Add<CheckDeserializedEntityBlock>().After<DeserializeEntityBlock>();
                }));

            services.RegisterAllCommands(assembly);
        }
    }
}