
namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Plugin.Sample.BizFx.DevOps.Commands;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = Plugin.Sample.BizFx.DevOps.Policies.PluginPolicy;

    [PipelineDisplayName("Dashboard")]
    public class Dashboard : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public Dashboard(
            CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "DevOps-Dashboard")
            {
                return entityView;
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            entityView.UiHint = "Flat";
            entityView.Icon = pluginPolicy.Icon;
            entityView.DisplayName = "Dev Ops";

            try
            {
                this._commerceCommander.Command<ChildViewEnvironmentDetails>().Process(context.CommerceContext, entityView, context.CommerceContext.Environment);

                await this._commerceCommander.Command<ChildViewResources>().Process(context.CommerceContext, entityView);

                await this._commerceCommander.Command<ChildViewEnvironments>().Process(context.CommerceContext, entityView);

                this._commerceCommander.Command<ChildViewDevOpsConnectionPaths>().Process(context.CommerceContext, entityView);
                this._commerceCommander.Command<ChildViewDevOpsTopologies>().Process(context.CommerceContext, entityView);
            }
            catch(Exception ex)
            {
                context.Logger.LogError(ex, "DevOps.DashBoard.Exception");
            }
            return entityView;
        }
    }
}
