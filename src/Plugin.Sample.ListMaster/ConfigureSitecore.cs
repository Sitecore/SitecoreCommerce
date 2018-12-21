using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Sample.ListMaster
{
    using global::Plugin.Sample.ListMaster.EntityViews;

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
                        .Add<FormAddList>().Before<IFormatEntityViewPipeline>()
                        .Add<FormPublishList>().Before<IFormatEntityViewPipeline>()
                        .Add<FormImportList>().Before<IFormatEntityViewPipeline>()
                        .Add<ViewManagedList>().Before<IFormatEntityViewPipeline>();
                })
                .ConfigurePipeline<IFormatEntityViewPipeline>(d =>
                {
                    d.Add<EnsureActions>().After<PopulateEntityViewActionsBlock>()
                        .Add<EnsurePluginActions>().After<PopulateEntityViewActionsBlock>();
                })
                .ConfigurePipeline<IDoActionPipeline>(
                    c =>
                    {
                        c.Add<DoActionAddList>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionClearAllLists>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionPublishList>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionImportList>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionDeleteList>().After<ValidateEntityVersionBlock>();
                    })); 

            services.RegisterAllCommands(assembly);
        }
    }
}