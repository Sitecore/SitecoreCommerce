
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Plugin.Sample.ExtendedConditions
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;

    public class ConfigureSitecore : IConfigureSitecore
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            
            services.RegisterAllPipelineBlocks(assembly);
            
            services.RegisterAllCommands(assembly);
        }
    }
}