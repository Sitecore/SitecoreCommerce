// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigureSitecore.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using global::Plugin.Sample.ShipEngine.Pipelines.Blocks;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Plugin.Carts;
using Sitecore.Commerce.Plugin.Fulfillment;
using Sitecore.Framework.Configuration;
using Sitecore.Framework.Pipelines.Definitions.Extensions;
using System.Reflection;

namespace Plugin.Sample.ShipEngine
{
    /// <summary>
    /// The configure sitecore class.
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

            // Configure pipelines
            services.Sitecore().Pipelines(config => config
               .ConfigurePipeline<ICalculateCartPipeline>(d =>
                    {
                        d.Add<ShipEngineCalculateCartFulfillmentBlock>().After<CalculateCartFulfillmentBlock>();
                    }, order: 1500)

                .ConfigurePipeline<ICalculateCartLinesPipeline>(d =>
                    {
                        d.Add<ShipEngineCalculateCartLineFulfillmentBlock>().After<CalculateCartLinesFulfillmentBlock>();
                    }, order: 1500)

                .ConfigurePipeline<IValidatePartyPipeline>(d =>
                    {
                        d.Add<ResolveAddressBlock>().After<ValidatePartyBlock>();
                    })
                );

            services.RegisterAllCommands(assembly);
        }
    }
}