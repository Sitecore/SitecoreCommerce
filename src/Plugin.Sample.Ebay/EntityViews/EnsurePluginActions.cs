namespace Plugin.Sample.Ebay.EntityViews
{
    using System.Threading.Tasks;

    using global::Plugin.Sample.Plugin.Enhancements;
    using global::Plugin.Sample.Plugin.Enhancements.Commands;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

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

            var pluginPolicy = context.GetPolicy<global::Plugin.Sample.Ebay.Policies.PluginPolicy>();
            var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();

            var userPluginOptions = await this._commerceCommander.Command<PluginCommander>().CurrentUserSettings(context.CommerceContext, this._commerceCommander);

            if (userPluginOptions.EnabledPlugins.Contains("Sitecore.Commerce.Plugin.Ebay"))
            {
                if (userPluginOptions.HasPolicy<global::Plugin.Sample.Ebay.Policies.PluginPolicy>())
                {
                    pluginPolicy = userPluginOptions.GetPolicy<global::Plugin.Sample.Ebay.Policies.PluginPolicy>();
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
                    Name = "Roles.EnablePlugin.Sitecore.Commerce.Plugin.Ebay",
                    DisplayName = "Enable Ebay Integration",
                    Description = "Enable Ebay",
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
                    Name = "Roles.DisablePlugin.Sitecore.Commerce.Plugin.Ebay",
                    DisplayName = "Disable Plugin (Sitecore.Commerce.Plugin.Ebay)",
                    Description = "Disable Plugin (Sitecore.Commerce.Plugin.Ebay)",
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
