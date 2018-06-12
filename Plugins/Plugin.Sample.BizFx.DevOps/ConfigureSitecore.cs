// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   The SamplePlugin startup class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.BizFx.DevOps
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
             .ConfigurePipeline<IGetEntityViewPipeline>(d =>
             {
                 d.Add<Dashboard>().Before<IFormatEntityViewPipeline>()
                  .Add<EntityViewAppService>().Before<IFormatEntityViewPipeline>()
                  .Add<FormEditPolicy>().Before<IFormatEntityViewPipeline>()
                  .Add<FormPullEnvironment>().Before<IFormatEntityViewPipeline>()
                  .Add<FormExportEnvironment>().Before<IFormatEntityViewPipeline>()
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
                })

                
            //Custom End

            );

            services.RegisterAllCommands(assembly);
        }
    }
}