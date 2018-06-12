// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Composer
{
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.BusinessUsers;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    /// <summary>
    /// The composer plugin configure sitecore class.
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
             .ConfigurePipeline<IBizFxNavigationPipeline>(c => c.Add<GetComposerNavigationViewBlock>().Before<IFormatEntityViewPipeline>())

             .ConfigurePipeline<IGetEntityViewPipeline>(
                c =>
                    {
                        c.Add<GetComposerDashboardViewBlock>().Before<IFormatEntityViewPipeline>()
                            .Add<GetEntityComposerViewsBlock>().Before<IFormatEntityViewPipeline>()
                            .Add<GetComposerAddChildViewBlock>().Before<IFormatEntityViewPipeline>()
                            .Add<GetComposerAddChildViewFromTemplateViewBlock>().Before<IFormatEntityViewPipeline>()
                            .Add<GetComposerEditViewBlock>().Before<IFormatEntityViewPipeline>()
                            .Add<GetComposerMakeTemplateViewBlock>().Before<IFormatEntityViewPipeline>()
                            .Add<GetComposerAddPropertyViewBlock>().Before<IFormatEntityViewPipeline>()
                            .Add<GetComposerRemovePropertyViewBlock>().Before<IFormatEntityViewPipeline>()
                            .Add<GetComposerAddMinMaxPropertyConstraintViewBlock>().Before<IFormatEntityViewPipeline>()
                            .Add<GetComposerAddSelectionOptionPropertyConstraintViewBlock>().Before<IFormatEntityViewPipeline>();
                    })

             .ConfigurePipeline<IFormatEntityViewPipeline>(c => c.Add<PopulateComposerEntityViewActionsBlock>().After<PopulateEntityViewActionsBlock>())

             .ConfigurePipeline<IPopulateEntityViewActionsPipeline>(
                c =>
                    {
                        c.Add<PopulateComposerTemplatesViewActionsBlock>().After<InitializeEntityViewActionsBlock>()
                            .Add<PopulateComposerViewActionsBlock>().After<InitializeEntityViewActionsBlock>();
                    })

             .ConfigurePipeline<IDoActionPipeline>(
                   c =>
                   {
                       c.Add<DoActionEnsureDefaultTemplatesBlock>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionClearTemplatesBlock>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionRemoveTemplateBlock>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionAddChildViewBlock>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionAddChildViewFromTemplateBlock>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionEditViewBlock>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionRemoveViewBlock>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionMakeTemplateBlock>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionAddPropertyBlock>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionRemovePropertyBlock>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionAddMinMaxPropertyConstrainBlock>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionAddSelectionOptionPropertyConstrainBlock>().After<ValidateEntityVersionBlock>();
                   })

              .ConfigurePipeline<IRunningPluginsPipeline>(c => c.Add<RegisteredPluginBlock>().After<RunningPluginsBlock>()));

            services.RegisterAllCommands(assembly);
        }
    }
}