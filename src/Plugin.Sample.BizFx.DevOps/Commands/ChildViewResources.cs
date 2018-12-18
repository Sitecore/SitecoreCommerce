namespace Plugin.Sample.BizFx.DevOps.Commands
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Plugin.Sample.BizFx.DevOps.Entities;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;

    using PluginPolicy = Plugin.Sample.BizFx.DevOps.Policies.PluginPolicy;

    public class ChildViewResources : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        public ChildViewResources(IServiceProvider serviceProvider,
            CommerceCommander commerceCommander) : base(serviceProvider)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public async Task<EntityView> Process(CommerceContext commerceContext, EntityView entityView)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var pluginPolicy = commerceContext.GetPolicy<PluginPolicy>();

                try
                {
                    var masterListEntityView = new EntityView
                    {
                        EntityId = string.Empty,
                        ItemId = string.Empty,
                        DisplayName = "Resources",
                        Name = "Resources",
                        UiHint = "Table",
                        Icon = pluginPolicy.Icon
                    };
                    entityView.ChildViews.Add(masterListEntityView);

                    var appServices = await this._commerceCommander.Command<ListCommander>().GetListItems<AppService>(commerceContext, "AppServices", 0, 999);

                    foreach (var appService in appServices)
                    {
                        var childView = new EntityView
                        {
                            EntityId = string.Empty,
                            ItemId = appService.Id,
                            DisplayName = appService.Name,
                            Name = appService.Name,
                            Icon = pluginPolicy.Icon
                        };
                        masterListEntityView.ChildViews.Add(childView);

                        childView.Properties.Add(new ViewProperty { Name = "Name", RawValue = appService.Name, UiType = "EntityLink" });
                        childView.Properties.Add(new ViewProperty { Name = "Type", RawValue = appService.ServiceType });

                        var hostProperty = new ViewProperty { Name = "Host", RawValue = appService.Host, UiType = "Html", OriginalType = "Html" };

                        var hostUrl = hostProperty.RawValue;

                        if (appService.ServiceType.Contains("CommerceEngine"))
                        {
                            if (appService.ServiceType.Contains("Azure"))
                            {
                                hostProperty.RawValue = $"<a href='http://{hostUrl + "/api/$metadata"}' target='_blank'>Commerce Engine</a> ";
                            }
                            else
                            {
                                hostUrl = hostUrl + "/api/$metadata";
                            }
                        }
                        else if (appService.ServiceType.Contains("Sitecore"))
                        {
                            hostProperty.RawValue = $"<a href='https://{hostUrl}' target='_blank'>(Shop) {hostUrl}</a>";
                            
                            if (appService.ServiceType == "Sitecore-CM")
                            {
                                hostProperty.RawValue = hostProperty.RawValue + $"<br><a href='https://{hostUrl}/sitecore' target='_blank'>(admin){hostUrl}/sitecore</a>";
                            }
                        }
                        else if (appService.ServiceType.Contains("Azure"))
                        {
                            if (appService.ServiceType == "Azure-AI")
                            {
                                hostProperty.RawValue = $"<a href='http://{hostUrl}' target='_blank'>Azure Application Insights</a> ";
                            }
                            else
                            {
                                hostProperty.RawValue = hostProperty.RawValue + $"<br><a href='https://portal.azure.com/#resource/subscriptions/4f5f5859-08fe-47cf-8a9e-4684708594c7/resourceGroups/yalsitecore/providers/Microsoft.Web/sites/yalsitecore-solutionstorefront-shop/appServices' target='_blank'>(azure){appService.Host}</a>";
                            }
                        }
                        else
                        {
                            hostProperty.RawValue = $"<a href='http://{hostUrl}' target='_blank'>{hostUrl}</a>";
                        }

                        childView.Properties.Add(hostProperty);
                    }
                }
                catch (Exception ex)
                {
                    commerceContext.Logger.LogError($"ChildViewEnvironments.Exception: Message={ex.Message}");
                }
                return entityView;
            }
        }
    }
}