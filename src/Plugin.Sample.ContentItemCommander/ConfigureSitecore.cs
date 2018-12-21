
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Sample.ContentItemCommander
{
    using Plugin.Sample.ContentItemCommander.EntityViews;

    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using Sitecore.Commerce.Plugin.BusinessUsers;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Management;

    using GetItemByIdBlock = Plugin.Sample.ContentItemCommander.Pipelines.Blocks.GetItemByIdBlock;
    using GetItemsByPathBlock = Plugin.Sample.ContentItemCommander.Pipelines.Blocks.GetItemsByPathBlock;

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
                .ConfigurePipeline<IGetItemByIdPipeline>(d =>
                {
                    d.Replace<Sitecore.Commerce.Plugin.Management.GetItemByIdBlock, GetItemByIdBlock>();
                })
                .ConfigurePipeline<IGetItemsByPathPipeline>(d =>
                {
                    d.Replace<Sitecore.Commerce.Plugin.Management.GetItemsByPathBlock, GetItemsByPathBlock>();
                }));

            services.RegisterAllCommands(assembly);
        }
    }
}