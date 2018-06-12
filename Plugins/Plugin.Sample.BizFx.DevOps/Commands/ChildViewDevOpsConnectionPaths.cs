// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChildViewDevOpsConnectionPaths.cs" company="Sitecore Corporation">
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
    /// Defines the ChildViewDevOpsConnectionPaths command.
    /// </summary>
    public class ChildViewDevOpsConnectionPaths : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChildViewDevOpsConnectionPaths"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public ChildViewDevOpsConnectionPaths(IServiceProvider serviceProvider,
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

                    var pathsView = new EntityView
                    {
                        EntityId = "",
                        ItemId = "Id",
                        DisplayName = "Connection Paths",
                        Name = "Connection Paths",
                        UiHint = "Table",
                        Icon = pluginPolicy.Icon
                    };

                    var nodeContext = this._commerceCommander.CurrentNodeContext(commerceContext);

                    AddAsLineItem(pathsView, new ViewProperty { Name = "WebRootPath", RawValue = nodeContext.WebRootPath }, pluginPolicy);
                    AddAsLineItem(pathsView, new ViewProperty { Name = "LoggingPath", RawValue = nodeContext.LoggingPath }, pluginPolicy);
                    AddAsLineItem(pathsView, new ViewProperty { Name = "BootstrapEnvironmentPath", RawValue = nodeContext.BootstrapEnvironmentPath }, pluginPolicy);
                    AddAsLineItem(pathsView, new ViewProperty { Name = "BootstrapProviderPath", RawValue = nodeContext.BootstrapProviderPath }, pluginPolicy);

                    entityView.ChildViews.Add(pathsView);
                }
                catch (Exception ex)
                {
                    commerceContext.Logger.LogError($"ChildViewEnvironments.Exception: Message={ex.Message}");
                }
                return entityView;
            }



        }

        private void AddAsLineItem(EntityView parentView, ViewProperty property, PluginPolicy pluginPolicy)
        {
            var pathsView1 = new EntityView
            {
                EntityId = "",
                ItemId = "Id",
                DisplayName = "Connection Paths",
                Name = "Connection Paths",
                UiHint = "List",
                Icon = pluginPolicy.Icon
            };
            parentView.ChildViews.Add(pathsView1);
            pathsView1.Properties
                .Add(new ViewProperty { Name = "Path", RawValue = property.Name });
            pathsView1.Properties
                .Add(property);
        }

    }
}