
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Sample.Composer.Tags
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using Sitecore.Commerce.Plugin.BusinessUsers;
    using Sitecore.Commerce.EntityViews;

    /// <summary>
    /// The configure sitecore class.  This allows a Plugin to wire up new Pipelines or to change existing ones.
    /// </summary>
    public class ConfigureSitecore : IConfigureSitecore
    {
        /// <summary>
        /// The configure services constructor.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            //Detects and registers any Pipeline blocks in the Plugi
            services.RegisterAllPipelineBlocks(assembly);

            //Manipulation of pipelines during startup
            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<IBizFxNavigationPipeline>(d =>
                    {
                        //d.Add<EnsureNavigationView>()
                        /*.Add<GetBusinessUsersNavigationViewBlock>()*/;
                    })
                .ConfigurePipeline<IGetEntityViewPipeline>(d =>
                    {
                        d
                         .Add<FomAddDashboardEntity>().Before<IFormatEntityViewPipeline>()
                        .Add<FormAddTagToTemplate>().Before<IFormatEntityViewPipeline>()
                        //.Add<EnsureBusinessUserEntityBlock>().After<GetEntityViewBlock>()
                        //.Add<GetBusinessUserDashboardViewBlock>().After<PopulateEntityVersionBlock>()
                        //.Add<GetAddTimeZoneViewBlock>().After<PopulateEntityVersionBlock>()
                        //.Add<GetEditTimeFormatsViewBlock>().After<PopulateEntityVersionBlock>()
                        .Add<EnsureTaggedViews>().After<PopulateEntityVersionBlock>()
                        .Add<EnsureTemplateTags>().Before<IFormatEntityViewPipeline>()
                        
                        .Add<EnsureConnectViews>().Before<IFormatEntityViewPipeline>();
                    })
                .ConfigurePipeline<IFormatEntityViewPipeline>(d =>
                    {
                        d.Add<EnsureActions>().After<PopulateEntityViewActionsBlock>();
                    })
                .ConfigurePipeline<IDoActionPipeline>(
                c =>
                    {
                        c.Add<DoActionAddDashboardEntity>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionRemoveDashboardEntity>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionAddAddTagToTemplate>().After<ValidateEntityVersionBlock>();
                    })
                //.ConfigurePipeline<IPopulateEntityViewActionsPipeline>(c =>
                //    {
                //        c
                //        .Add<PopulateBusinessUserDashboardViewActionsBlock>().After<InitializeEntityViewActionsBlock>()
                //        .Add<PopulateTimeZonesViewActionsBlock>().After<InitializeEntityViewActionsBlock>();
                //    })
            );

            //Detects and registers all commands in the Plugin
            services.RegisterAllCommands(assembly);
        }
    }
}