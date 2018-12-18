namespace Plugin.Sample.BizFx.DevOps.Commands
{
    using System;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;

    using PluginPolicy = Plugin.Sample.BizFx.DevOps.Policies.PluginPolicy;

    public class ChildViewDevOpsConnectionPaths : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        public ChildViewDevOpsConnectionPaths(IServiceProvider serviceProvider,
            CommerceCommander commerceCommander) : base(serviceProvider)
        {
            this._commerceCommander = commerceCommander;
        }

        public EntityView Process(CommerceContext commerceContext, EntityView entityView)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var pluginPolicy = commerceContext.GetPolicy<PluginPolicy>();

                try
                {
                    var pathsView = new EntityView
                    {
                        EntityId = string.Empty,
                        ItemId = "Id",
                        DisplayName = "Connection Paths",
                        Name = "Connection Paths",
                        UiHint = "Table",
                        Icon = pluginPolicy.Icon
                    };

                    var nodeContext = this._commerceCommander.CurrentNodeContext(commerceContext);

                    this.AddAsLineItem(pathsView, new ViewProperty { Name = "WebRootPath", RawValue = nodeContext.WebRootPath }, pluginPolicy);
                    this.AddAsLineItem(pathsView, new ViewProperty { Name = "LoggingPath", RawValue = nodeContext.LoggingPath }, pluginPolicy);
                    this.AddAsLineItem(pathsView, new ViewProperty { Name = "BootstrapEnvironmentPath", RawValue = nodeContext.BootstrapEnvironmentPath }, pluginPolicy);
                    this.AddAsLineItem(pathsView, new ViewProperty { Name = "BootstrapProviderPath", RawValue = nodeContext.BootstrapProviderPath }, pluginPolicy);

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
                EntityId = string.Empty,
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