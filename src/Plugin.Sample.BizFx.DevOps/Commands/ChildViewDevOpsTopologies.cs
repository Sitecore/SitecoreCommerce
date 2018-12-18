namespace Plugin.Sample.BizFx.DevOps.Commands
{
    using System;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;

    using PluginPolicy = Plugin.Sample.BizFx.DevOps.Policies.PluginPolicy;

    public class ChildViewDevOpsTopologies : CommerceCommand
    {
        public EntityView Process(CommerceContext commerceContext, EntityView entityView)
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
                        DisplayName = "topologies View",
                        Name = "TopologiesView",
                        UiHint = "List",
                        Icon = pluginPolicy.Icon
                    };
                    entityView.ChildViews.Add(masterListEntityView);

                    var childView = new EntityView
                    {
                        EntityId = string.Empty,
                        ItemId = "Topology01",
                        DisplayName = "Topology01",
                        Name = "Topology01",
                        Icon = pluginPolicy.Icon
                    };

                    childView.Properties.Add(new ViewProperty { Name = "Name", RawValue = "Topology01" });
                    childView.Properties.Add(new ViewProperty { Name = "Type", RawValue = "OneBox" });
                    childView.Properties.Add(new ViewProperty { Name = "Description", RawValue = "OneBox Topology" });

                    masterListEntityView.ChildViews.Add(childView);

                    var childView2 = new EntityView
                    {
                        EntityId = string.Empty,
                        ItemId = "Topology02",
                        DisplayName = "Topology02",
                        Name = "Topology02",
                        Icon = pluginPolicy.Icon
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