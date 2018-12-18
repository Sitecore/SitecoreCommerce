
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Sample.VatTax
{
    using Plugin.Sample.VatTax.EntityViews;
    using Plugin.Sample.VatTax.Pipelines.Blocks;

    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using Sitecore.Commerce.Plugin.BusinessUsers;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Carts;
    
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
                .ConfigurePipeline<ICalculateCartLinesPipeline>(d =>
                {
                    d.Replace<Sitecore.Commerce.Plugin.Tax.CalculateCartLinesTaxBlock, CalculateCartLinesTaxBlock>();
                })
                .ConfigurePipeline<IGetEntityViewPipeline>(d =>
                {
                    d.Add<Dashboard>().Before<IFormatEntityViewPipeline>()
                        .Add<FormAddDashboardEntity>().Before<IFormatEntityViewPipeline>();
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
                    }));
            
            services.RegisterAllCommands(assembly);
        }
    }
}