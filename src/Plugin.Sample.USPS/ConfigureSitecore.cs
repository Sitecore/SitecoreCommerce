using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Plugin.Sample.USPS.Pipelines.Blocks;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;
using System.Reflection;

namespace Plugin.Sample.USPS
{
    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            services.RegisterAllPipelineBlocks(assembly);

            services.Sitecore().Pipelines(config => config
                .ConfigurePipeline<IValidatePartyPipeline>(d =>
                {
                    d.Add<ResolveAddressBlock>().After<ValidatePartyBlock>();
                })
            );

            services.RegisterAllCommands(assembly);
        }
    }
}
