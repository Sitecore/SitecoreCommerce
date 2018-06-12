// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChildViewResources.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   Defines the ChildViewResources command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.BizFx.DevOps
{
    using System;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.Plugin.Management;
    using Microsoft.Extensions.Logging;
    using Sitecore.Services.Core.Model;
    using Sitecore.Commerce.EntityViews;

    /// <summary>
    /// Defines the ChildViewResources command.
    /// </summary>
    public class ChildViewResources : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildViewResources"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public ChildViewResources(IServiceProvider serviceProvider,
            CommerceCommander commerceCommander) : base(serviceProvider)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// The process of the command
        /// </summary>
        /// <param name="commerceContext">
        /// The commerce context
        /// </param>
        /// <param name="entityView">
        /// The entityView for the command
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<EntityView> Process(CommerceContext commerceContext, EntityView entityView)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                var pluginPolicy = commerceContext.GetPolicy<PluginPolicy>();

                try
                {
                    var masterListEntityView = new EntityView
                    {
                        EntityId = "",
                        ItemId = "",
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
                            EntityId = "",
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
                                //hostProperty.RawValue = $"<a href='http://{hostUrl + "/api/$metadata"}' target='_blank'><img alt='Commerce Engine' height=30 width=30 src='/assets/images/Azure/Azure App Service - Web App_COLOR.png' style=''/> Commerce Engine</a> ";
                                hostProperty.RawValue = $"<a href='http://{hostUrl + "/api/$metadata"}' target='_blank'>Commerce Engine</a> ";

                            }
                            else
                            {
                                hostUrl = hostUrl + "/api/$metadata";
                            }

                                
                        }
                        else if (appService.ServiceType.Contains("Sitecore"))
                        {
                            //hostProperty.RawValue = $"<a href='https://{hostUrl}' target='_blank'><img alt='Azure Application Insights' height=30 width=30 src='/assets/images/Azure/Azure App Service - Web App_COLOR.png' style=''/>  (Shop){hostUrl}</a>";
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
                                //hostProperty.RawValue = @"<svg id=""application - insights"" xmlns=""http://www.w3.org/2000/svg"" version=""1.1"" viewBox=""0 0 50 50"" width=""100 % "" height=""100 % ""> <path fill=""#68217A"" d=""M41.4,14.7L41.4,14.7v-0.3c0-7.7-6.6-14.1-14.7-14.2c-0.2-0.3-4.8,0.1-4.8,0.1l0,0c-7.3,0.9-13,7-13,14.1 c0,0.2-0.8,5.8,4.9,10.5c2.6,2.3,5.3,8.5,5.7,10.3l0.3,0.6h10.6l0.3-0.6c0.4-1.8,3.2-8,5.7-10.2C42.1,20.2,41.4,14.9,41.4,14.7z""></path> <rect x=""20"" y=""39.4"" fill=""#7A7A7A"" width=""10.6"" height=""3.4""></rect> <polygon fill=""#7A7A7A"" points=""23.3,50 27.2,50 30.5,46.5 20,46.5 ""></polygon> <g opacity=""0.65""> <path fill=""#FFFFFF"" d=""M27.9,35.3h-2V22.6l-1.7,0v12.6h-2V22.6l-1.7,0c-2,0-3.7-1.7-3.7-3.7s1.6-3.7,3.7-3.7s3.7,1.7,3.7,3.7v1.7 l1.7,0v-1.7c0-2,1.7-3.7,3.7-3.7s3.7,1.7,3.7,3.7c0,2-1.7,3.7-3.7,3.7l-1.7,0V35.3z M20.5,17.2c-0.9,0-1.7,0.7-1.7,1.7 c0,0.9,0.8,1.7,1.7,1.7l1.7,0v-1.7C22.1,18,21.4,17.2,20.5,17.2z M29.6,17.2c-0.9,0-1.7,0.8-1.7,1.7v1.7l1.7,0 c0.9,0,1.7-0.8,1.7-1.7C31.3,18,30.6,17.2,29.6,17.2z""></path> </g> <g opacity=""0.15""> <path fill=""#FFFFFF"" d=""M26.7,0.2c-0.2-0.3-4.8,0.1-4.8,0.1c-7.3,0.9-13,7-13,14.1c0,0.2-0.7,5.1,3.9,9.6L34.4,2.4 C32.1,1,29.5,0.2,26.7,0.2z""></path> </g> </svg>
                                //";

                                //hostProperty.RawValue = "<img alt='This is the alternate' height=100 width=100 src='http://scbizfx.westus.cloudapp.azure.com/-/media/Images/Habitat/6042177_01.ashx?h=625&w=770' style=''/>";
                                //hostProperty.RawValue = $"<a href='http://{hostUrl}' target='_blank'><img alt='Azure Application Insights' height=30 width=30 src='/assets/images/Azure/AzureApplicationInsights.png' style=''/>  Azure Application Insights</a> ";
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