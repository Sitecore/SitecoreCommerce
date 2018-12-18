
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Sample.HandleMissingSitecore
{
    using Plugin.Sample.HandleMissingSitecore.Pipelines.Blocks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Shops;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            services.RegisterAllPipelineBlocks(assembly);
            
            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<IValidateContextPipeline>(d =>
                {
                    d.Remove<ValidateShopBlock>()
                        .Add<EnsureDefaultShop>().Before<ValidateShopCurrencyBlock>();
                })
                .ConfigurePipeline<IGetShopPipeline>(d =>
                {
                    d.Add<EnsureDefaultShop>().After<GetShopBlock>();
                }));
            
            services.RegisterAllCommands(assembly);
        }
    }
}