
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Sample.Orders.Enhancements
{
    using global::Plugin.Sample.Orders.Enhancements.EntityViews;
    using global::Plugin.Sample.Orders.Enhancements.Pipelines.Blocks;

    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Commerce.Plugin.Carts;

    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            services.RegisterAllPipelineBlocks(assembly);
            
            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<IGetEntityViewPipeline>(d =>
                    {
                        d.Add<ViewOrders>().Before<IFormatEntityViewPipeline>()
                         .Add<ViewMessagesDashboard>().Before<IFormatEntityViewPipeline>()
                         .Add<ViewOrderMoveLinesUp>().Before<IFormatEntityViewPipeline>();
                    })
                .ConfigurePipeline<IFormatEntityViewPipeline>(d =>
                    {
                        d.Add<EnsureActionOrders>().After<PopulateEntityViewActionsBlock>();
                    })
                .ConfigurePipeline<IDoActionPipeline>(
                    c =>
                    {
                        c.Add<DoActionMonitorTotals>().After<ValidateEntityVersionBlock>()
                            .Add<DoActionClearTotals>().After<ValidateEntityVersionBlock>();
                    })
                .ConfigurePipeline<ICreateOrderPipeline>(d =>
                    {
                        d.Add<FailedOrderMessageBlock>().Before<CreateOrderBlock>();
                    })
                .ConfigurePipeline<IAddCartLinePipeline>(d =>
                    {
                        d.Add<FailUnavailableItemAdded>();
                    }));
            
            services.RegisterAllCommands(assembly);
        }
    }
}