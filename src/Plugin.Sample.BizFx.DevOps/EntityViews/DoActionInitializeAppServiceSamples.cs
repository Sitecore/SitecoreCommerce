namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using Plugin.Sample.BizFx.DevOps.Entities;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionInitializeAppServiceSamples")]
    public class DoActionInitializeAppServiceSamples : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionInitializeAppServiceSamples(
            CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("DevOps-InitializeSampleAppServices", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            await this.AddAppService("localhost", "CommerceEngine-IIS", "localhost:5000", "Locally running Commerce Engine solution", context.CommerceContext);

            await this.AddAppService("kha902-Shops", "CommerceEngine-Azure", "khac902-solutionstorefront-shop.azurewebsites.net", "khac902 Commerce Engine", context.CommerceContext);
            await this.AddAppService("kha902-CM", "Sitecore-CM", "khac902-cm.azurewebsites.net", "khac902-CM Sitecore", context.CommerceContext);
            await this.AddAppService("kha902-CD", "Sitecore-CD", "khac902-cd.azurewebsites.net", "khac902-CD Sitecore", context.CommerceContext);
            await this.AddAppService("kha902-AI", "Azure-AI", "portal.azure.com/#resource/subscriptions/4f5f5859-08fe-47cf-8a9e-4684708594c7/resourceGroups/khaC902/providers/Microsoft.Insights/components/khac902-ai/overview", "khac902-Sitecore Commerce Application Insights", context.CommerceContext);

            await this.AddAppService("YAL01-Shops", "CommerceEngine-Azure", "yalsitecore-solutionstorefront-shop.azurewebsites.net", "YAL01 Commerce Engine", context.CommerceContext);
            await this.AddAppService("YAL01-CM", "Sitecore-CM", "yalsitecore-cm.azurewebsites.net", "YAL01-CM", context.CommerceContext);
            await this.AddAppService("YAL01-CD", "Sitecore-CD", "yalsitecore-cd.azurewebsites.net", "YAL01-CD", context.CommerceContext);
            
            return entityView;
        }
        
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

            await this._commerceCommander.PersistEntity(commerceContext, appService);
            return "";
        }
    }
}
