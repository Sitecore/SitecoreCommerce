namespace Plugin.Sample.Plugin.Enhancements.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Plugin.Enhancements.Commands;

    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionDisablePlugin")]
    public class DoActionDisablePlugin : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionDisablePlugin(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Contains("Roles.DisablePlugin"))
            {
                return entityView;
            }

            try
            {
                var pluginName = entityView.Action.Replace("Roles.DisablePlugin.", "");

                var userPluginOptions = await this._commerceCommander.Command<PluginCommander>().CurrentUserSettings(context.CommerceContext, this._commerceCommander);

                userPluginOptions.EnabledPlugins.Remove(pluginName);

                await this._commerceCommander.PersistEntity(context.CommerceContext, userPluginOptions);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionAddDashboardEntity.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
