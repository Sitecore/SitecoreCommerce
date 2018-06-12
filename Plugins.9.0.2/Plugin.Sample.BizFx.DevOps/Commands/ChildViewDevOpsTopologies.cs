// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChildViewDevOpsTopologies.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// <summary>
//   Defines the ChildViewEnvironmentConnectionPaths command.
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
    /// Defines the ChildViewDevOpsTopologies command.
    /// </summary>
    public class ChildViewDevOpsTopologies : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildViewDevOpsTopologies"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public ChildViewDevOpsTopologies(IServiceProvider serviceProvider,
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
        public EntityView Process(CommerceContext commerceContext, EntityView entityView)
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
                        DisplayName = "topologies View",
                        Name = "TopologiesView",
                        UiHint = "List",
                        Icon = pluginPolicy.Icon
                    };
                    entityView.ChildViews.Add(masterListEntityView);

                    var childView = new EntityView
                    {
                        EntityId = "",
                        ItemId = "Topology01",
                        DisplayName = "Topology01",
                        Name = "Topology01",
                        Icon = pluginPolicy.Icon
                        //Policies = new List<Policy>() { new ViewImagePolicy { ImageUrl = @".\src\images\NetworkV2\48x48\server_to_client.png" } }
                    };

                    childView.Properties.Add(new ViewProperty { Name = "Name", RawValue = "Topology01" });
                    childView.Properties.Add(new ViewProperty { Name = "Type", RawValue = "OneBox" });
                    childView.Properties.Add(new ViewProperty { Name = "Description", RawValue = "OneBox Topology" });

                    masterListEntityView.ChildViews.Add(childView);

                    var childView2 = new EntityView
                    {
                        EntityId = "",
                        ItemId = "Topology02",
                        DisplayName = "Topology02",
                        Name = "Topology02",
                        Icon = pluginPolicy.Icon
                        //Policies = new List<Policy>() { new ViewImagePolicy { ImageUrl = @".\src\images\NetworkV2\48x48\server_to_client.png" } }
                    };

                    childView2.Properties.Add(new ViewProperty { Name = "Name", RawValue = "Topology02" });
                    childView2.Properties.Add(new ViewProperty { Name = "Type", RawValue = "SimpleHa" });
                    childView2.Properties.Add(new ViewProperty { Name = "Description", RawValue = "Simple HA configuration" });

                    masterListEntityView.ChildViews.Add(childView2);

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