
namespace Plugin.Sample.JsonCommander
{
    using System.Reflection;    
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;

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
                .ConfigurePipeline<IFindEntityPipeline>(d =>
                {
                    //d.Remove<LoadLocalizationEntityBlock>()
                    // .Remove<LoadEntityLocalizedPropertiesBlock>();
                })
                .ConfigurePipeline<IPersistEntityPipeline>(d =>
                {
                    //d.Remove<LocalizeEntityPropertiesBlock>();
                })
            );

            services.RegisterAllCommands(assembly);
        }
    }
}