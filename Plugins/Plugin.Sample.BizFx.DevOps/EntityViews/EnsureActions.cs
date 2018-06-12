// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnsureActions.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.BizFx.DevOps
{
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.BusinessUsers;

    //using Sitecore.Commerce.Plugin.BusinessUsers.Extensions;

    /// <summary>
    /// Defines a block which validates, inserts and updates Actions for this Plugin.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("EnsureActions")]
    public class EnsureActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsureActions"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EnsureActions(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            if (entityView.Name == "BusinessUserDashboard")
            {
                var businessUser = await this._commerceCommander.Command<BusinessUserCommander>().CurrentBusinessUser(context.CommerceContext);

                //if (!businessUser.GetPolicy<UserPluginsPolicy>().PlugIns.Any(p => p.PolicyId == this.GetType().Namespace))
                //{
                    var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();
                    actionsPolicy.Actions.Add(new EntityActionView
                    {
                        Name = "BusinessUser-EnablePlugin",
                        DisplayName = "Enable DevOps",
                        Description = "",
                        IsEnabled = true,
                        RequiresConfirmation = true,
                        EntityView = "",
                        UiHint = "",
                        Icon = pluginPolicy.Icon
                    });
                    entityView.ItemId = this.GetType().Namespace;
                    return entityView;
                //}
            }

            var policyEntityViews = entityView.ChildViews.Where(p => p.Name.Contains("Policy"));

            foreach (var policyEntityView in policyEntityViews)
            {
                var flatEntityViewActionsPolicy = policyEntityView.GetPolicy<ActionsPolicy>();

                flatEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-EditPolicy",
                    DisplayName = "Edit Policy",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "DevOps-EditPolicy",
                    UiHint = "",
                    Icon = pluginPolicy.Icon
                });
            }

            if (entityView.Name != "DevOps-Dashboard")
            {
                return entityView;
            }

            var mainActionsPolicy = entityView.GetPolicy<ActionsPolicy>();

            mainActionsPolicy.Actions.Add(new EntityActionView
            {
                Name = "DevOps-CleanCurrentEnvironment",
                DisplayName = "Clean Current Environment",
                Description = "",
                IsEnabled = true,
                RequiresConfirmation = true,
                EntityView = "",
                UiHint = "",
                Icon = pluginPolicy.Icon
            });
            mainActionsPolicy.Actions.Add(new EntityActionView
            {
                Name = "DevOps-InitializeCurrentEnvironment",
                DisplayName = "Initialize Current Environment",
                Description = "",
                IsEnabled = true,
                RequiresConfirmation = true,
                EntityView = "",
                UiHint = "",
                Icon = pluginPolicy.Icon
            });

            //var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();

            var tableEntityView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Resources");

            if (tableEntityView != null)
            {
                var tableEntityViewActionsPolicy = tableEntityView.GetPolicy<ActionsPolicy>();

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-BootStrapAppService",
                    DisplayName = "Bootstrap Resources",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "",
                    UiHint = "",
                    Icon = pluginPolicy.Icon
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-CleanEnvironment",
                    DisplayName = "Clean Environment",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "",
                    UiHint = "",
                    Icon = pluginPolicy.Icon
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-InitializeEnvironment",
                    DisplayName = "Initialize Environment",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "",
                    UiHint = "",
                    Icon = pluginPolicy.Icon
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-PullEnvironment",
                    DisplayName = "Pull Environment",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "DevOps-PullEnvironment",
                    UiHint = "",
                    Icon = pluginPolicy.Icon
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-PushEnvironment",
                    DisplayName = "Push Environment",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "DevOps-PushEnvironment",
                    UiHint = "",
                    Icon = pluginPolicy.Icon
                });

                //This action creates an Action that allows you to show a another EntityView
                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-AddAppService",
                    DisplayName = "Add Resource",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "",
                    UiHint = "",
                    Icon = pluginPolicy.Icon
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-RemoveAppService",
                    DisplayName = "Remove Resource",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "",
                    UiHint = "",
                    Icon = pluginPolicy.Icon
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-InitializeSampleAppServices",
                    DisplayName = "Ensure Sample Resources",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = "",
                    UiHint = "",
                    Icon = pluginPolicy.Icon
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-ClearAppServices",
                    DisplayName = "Clear Resources",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = "",
                    UiHint = "",
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
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = "DevOps-CloneEnvironment",
                    UiHint = "",
                    Icon = "newtons_cradle"
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-ExportEnvironment",
                    DisplayName = "Export Environment",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "DevOps-ExportEnvironment",
                    UiHint = "",
                    Icon = "escalator_up"
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "DevOps-RemoveEnvironment",
                    DisplayName = "Remove Environment",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "DevOps-RemoveEnvironment",
                    UiHint = "",
                    Icon = "delete"
                });
            }

            return entityView;
        }
    }
}
