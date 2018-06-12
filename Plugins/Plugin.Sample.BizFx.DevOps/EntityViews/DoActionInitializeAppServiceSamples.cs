// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DoActionInitializeAppServiceSamples.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.BizFx.DevOps
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Sitecore.Framework.Pipelines;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.ManagedLists;

    /// <summary>
    /// Defines the do action edit line item block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("DoActionInitializeAppServiceSamples")]
    public class DoActionInitializeAppServiceSamples : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionInitializeAppServiceSamples"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionInitializeAppServiceSamples(
            CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="entityView">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="EntityView"/>.
        /// </returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {

            if (entityView == null
                || !entityView.Action.Equals("DevOps-InitializeSampleAppServices", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            await AddAppService("localhost", "CommerceEngine-IIS", "localhost:5000", "Locally running Commerce Engine solution", context.CommerceContext);

            await AddAppService("kha902-Shops", "CommerceEngine-Azure", "khac902-solutionstorefront-shop.azurewebsites.net", "khac902 Commerce Engine", context.CommerceContext);
            await AddAppService("kha902-CM", "Sitecore-CM", "khac902-cm.azurewebsites.net", "khac902-CM Sitecore", context.CommerceContext);
            await AddAppService("kha902-CD", "Sitecore-CD", "khac902-cd.azurewebsites.net", "khac902-CD Sitecore", context.CommerceContext);
            await AddAppService("kha902-AI", "Azure-AI", "portal.azure.com/#resource/subscriptions/4f5f5859-08fe-47cf-8a9e-4684708594c7/resourceGroups/khaC902/providers/Microsoft.Insights/components/khac902-ai/overview", "khac902-Sitecore Commerce Application Insights", context.CommerceContext);

            await AddAppService("YAL01-Shops", "CommerceEngine-Azure", "yalsitecore-solutionstorefront-shop.azurewebsites.net", "YAL01 Commerce Engine", context.CommerceContext);
            await AddAppService("YAL01-CM", "Sitecore-CM", "yalsitecore-cm.azurewebsites.net", "YAL01-CM", context.CommerceContext);
            await AddAppService("YAL01-CD", "Sitecore-CD", "yalsitecore-cd.azurewebsites.net", "YAL01-CD", context.CommerceContext);



            return entityView;
        }

        /// <summary>
        /// Add Resource
        /// </summary>
        /// <param name="appServiceName">The name of the AppService</param>
        /// <param name="type">The Type of AppService </param>
        /// <param name="host">The Host for the AppService</param>
        /// <param name="description">A Description fro the AppService</param>
        /// <param name="commerceContext">A CommerceContext</param>
        /// <returns>A string</returns>
        public async Task<string> AddAppService(string appServiceName, string type, string host, string description, CommerceContext commerceContext)
        {

            var appService = new AppService
            {
                Id = $"Entity-AppService-{appServiceName}",
                Name = appServiceName,
                ServiceType = type,
                Host = host,
                Description = description
            };

            appService.GetComponent<ListMembershipsComponent>().Memberships.Add("AppServices");

            var result = await this._commerceCommander.PersistEntity(commerceContext, appService);
            return "";

        }

    }
}
