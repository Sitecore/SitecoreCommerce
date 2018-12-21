namespace Plugin.Sample.Search.Management.EntityViews
{
    using System.Threading.Tasks;

    using global::Plugin.Sample.Plugin.Enhancements.Commands;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = global::Plugin.Sample.Search.Management.Policies.PluginPolicy;
    [PipelineDisplayName("EnsurePluginActions")]
    public class EnsurePluginActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public EnsurePluginActions(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (!string.IsNullOrEmpty(entityView.Action))
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();
            var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();

            var userPluginOptions = await this._commerceCommander
                .Command<PluginCommander>()
                .CurrentUserSettings(context.CommerceContext, 
                    this._commerceCommander);

            if (userPluginOptions.EnabledPlugins
                .Contains("Plugin.Sample.Search.Management"))
            {
                if (userPluginOptions.HasPolicy<PluginPolicy>())
                {
                    pluginPolicy = userPluginOptions.GetPolicy<PluginPolicy>();
                }
                else
                {
                    pluginPolicy.IsDisabled = false;
                }
            }
            else
            {
                pluginPolicy.IsDisabled = true;
            }

            if (pluginPolicy.IsDisabled)
            {
                actionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "Roles.EnablePlugin.Plugin.Sample.Search.Management",
                    DisplayName = "Enable Search Management",
                    Description = "Enable Search.Management",
                    IsEnabled = true, ConfirmationMessage = "This is where a confirmation message goes",
                    
                    RequiresConfirmation = true,
                    EntityView = string.Empty,
                    Icon = "box_into"
                });
            }
            else
            {
                actionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "Roles.DisablePlugin.Plugin.Sample.Search.Management",
                    DisplayName = "Disable Plugin (Plugin.Sample.Search.Management)",
                    Description = "Disable Plugin (Plugin.Sample.Search.Management)",
                    IsEnabled = true,
                    ConfirmationMessage = "This is where a confirmation message goes",

                    RequiresConfirmation = true,
                    EntityView = string.Empty,
                    Icon = "box_out"
                });
            }

            return entityView;
        }
    }
}
