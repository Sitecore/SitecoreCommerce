namespace Plugin.Sample.BizFx.DevOps.Commands
{
    using System;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;

    using PluginPolicy = Plugin.Sample.BizFx.DevOps.Policies.PluginPolicy;
    
    public class ChildViewEnvironmentDetails : CommerceCommand
    {
        private readonly CommerceCommander _commerceCommander;

        public ChildViewEnvironmentDetails(IServiceProvider serviceProvider,
            CommerceCommander commerceCommander) : base(serviceProvider)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public EntityView Process(CommerceContext commerceContext, EntityView entityView, CommerceEnvironment environment)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                try
                {
                    if (environment == null)
                    {
                        return entityView;
                    }

                    entityView.Properties
                        .Add(new ViewProperty { Name = "Environment", RawValue = environment.Name, UiType = "EntityLink" });
                    entityView.Properties
                        .Add(new ViewProperty { Name = "Policies", RawValue = environment.Policies.Count });
                    entityView.Properties
                        .Add(new ViewProperty { Name = "Components", RawValue = environment.Components.Count });
                    entityView.Properties
                        .Add(new ViewProperty { Name = "ArtifactStoreId", RawValue = environment.ArtifactStoreId.ToString("N") });
                    entityView.Properties
                        .Add(new ViewProperty { Name = "GlobalEnvironmentName", RawValue = this._commerceCommander.CurrentNodeContext(commerceContext).GlobalEnvironmentName });
                    entityView.GetPolicy<ActionsPolicy>().Actions.Add(
                       new EntityActionView
                       {
                           Name = "DoAction",
                           IsEnabled = true,
                           Description = "Description",
                           DisplayName = "Do Action",
                           RequiresConfirmation = false,
                           UiHint = "RelatedList",
                           EntityView = "/entityView/GetListView-CompletedOrders"
                       });
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