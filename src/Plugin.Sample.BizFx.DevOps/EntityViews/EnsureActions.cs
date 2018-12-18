namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("EnsureActions")]
    public class EnsureActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{Name}: The argument cannot be null");

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            if (entityView.Name == "BusinessUserDashboard")
            {
                var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();
                actionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "BusinessUser-EnablePlugin",
                    DisplayName = "Enable DevOps",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = string.Empty,
                    UiHint = string.Empty,
                    Icon = pluginPolicy.Icon
                });
                entityView.ItemId = GetType().Namespace;
                return Task.FromResult(entityView);
            }

            var policyEntityViews = entityView.ChildViews.Where(p => p.Name.Contains("Policy"));

            foreach (var policyEntityView in policyEntityViews)
            {
                var flatEntityViewActionsPolicy = policyEntityView.GetPolicy<ActionsPolicy>();

                flatEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-EditPolicy",
                    DisplayName = "Edit Policy",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "DevOps-EditPolicy",
                    UiHint = string.Empty,
                    Icon = pluginPolicy.Icon
                });
            }

            if (entityView.Name != "DevOps-Dashboard")
            {
                return Task.FromResult(entityView);
            }

            var mainActionsPolicy = entityView.GetPolicy<ActionsPolicy>();

            mainActionsPolicy.Actions.Add(new EntityActionView
            {
                Name = "DevOps-CleanCurrentEnvironment",
                DisplayName = "Clean Current Environment",
                Description = string.Empty,
                IsEnabled = true,
                RequiresConfirmation = true,
                EntityView = string.Empty,
                UiHint = string.Empty,
                Icon = pluginPolicy.Icon
            });
            mainActionsPolicy.Actions.Add(new EntityActionView
            {
                Name = "DevOps-InitializeCurrentEnvironment",
                DisplayName = "Initialize Current Environment",
                Description = string.Empty,
                IsEnabled = true,
                RequiresConfirmation = true,
                EntityView = string.Empty,
                UiHint = string.Empty,
                Icon = pluginPolicy.Icon
            });

            var tableEntityView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Resources");

            if (tableEntityView != null)
            {
                var tableEntityViewActionsPolicy = tableEntityView.GetPolicy<ActionsPolicy>();

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-BootStrapAppService",
                    DisplayName = "Bootstrap Resources",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = string.Empty,
                    UiHint = string.Empty,
                    Icon = pluginPolicy.Icon
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-CleanEnvironment",
                    DisplayName = "Clean Environment",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = string.Empty,
                    UiHint = string.Empty,
                    Icon = pluginPolicy.Icon
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-InitializeEnvironment",
                    DisplayName = "Initialize Environment",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = string.Empty,
                    UiHint = string.Empty,
                    Icon = pluginPolicy.Icon
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-PullEnvironment",
                    DisplayName = "Pull Environment",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "DevOps-PullEnvironment",
                    UiHint = string.Empty,
                    Icon = pluginPolicy.Icon
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-PushEnvironment",
                    DisplayName = "Push Environment",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "DevOps-PushEnvironment",
                    UiHint = string.Empty,
                    Icon = pluginPolicy.Icon
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-AddAppService",
                    DisplayName = "Add Resource",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = string.Empty,
                    UiHint = string.Empty,
                    Icon = pluginPolicy.Icon
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-RemoveAppService",
                    DisplayName = "Remove Resource",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = string.Empty,
                    UiHint = string.Empty,
                    Icon = pluginPolicy.Icon
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-InitializeSampleAppServices",
                    DisplayName = "Ensure Sample Resources",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = string.Empty,
                    UiHint = string.Empty,
                    Icon = pluginPolicy.Icon
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-ClearAppServices",
                    DisplayName = "Clear Resources",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = string.Empty,
                    UiHint = string.Empty,
                    Icon = pluginPolicy.Icon
                });
            }

            var environmentsEntityView = entityView.ChildViews.FirstOrDefault(p => p.Name == "DevOps-ListView-Environments");

            if (environmentsEntityView != null)
            {
                var tableEntityViewActionsPolicy = environmentsEntityView.GetPolicy<ActionsPolicy>();

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-CloneEnvironment",
                    DisplayName = "Clone Environment",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = "DevOps-CloneEnvironment",
                    UiHint = string.Empty,
                    Icon = "newtons_cradle"
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-ExportEnvironment",
                    DisplayName = "Export Environment",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "DevOps-ExportEnvironment",
                    UiHint = string.Empty,
                    Icon = "escalator_up"
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-RemoveEnvironment",
                    DisplayName = "Remove Environment",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "DevOps-RemoveEnvironment",
                    UiHint = string.Empty,
                    Icon = "delete"
                });
            }

            return Task.FromResult(entityView);
        }
    }
}
