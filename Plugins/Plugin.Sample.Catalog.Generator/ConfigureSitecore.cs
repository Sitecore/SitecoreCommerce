// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   The SamplePlugin startup class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.CatalogGenerator
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Configuration;
    using Sitecore.Framework.Pipelines.Definitions.Extensions;
    using Sitecore.Commerce.EntityViews;

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

            // Custom Start
            .ConfigurePipeline<IGetEntityViewPipeline>(d =>
            {
                d.Add<FormGenerateSampleCatalog>().Before<IFormatEntityViewPipeline>();
            })
            .ConfigurePipeline<IFormatEntityViewPipeline>(d =>
            {
                d.Add<EnsureActions>().After<PopulateEntityViewActionsBlock>();
            })
            .ConfigurePipeline<IDoActionPipeline>(d =>
            {
                d.Add<DoActionGenerateSampleCatalog>().After<ValidateEntityVersionBlock>();
            })
            //Custom End

            .ConfigurePipeline<IConfigureServiceApiPipeline>(d => 
            d.Add<ConfigureServiceApiBlock>())
            );

            services.RegisterAllCommands(assembly);
        }
    }
}