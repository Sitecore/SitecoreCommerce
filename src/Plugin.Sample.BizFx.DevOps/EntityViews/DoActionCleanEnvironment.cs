namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using Plugin.Sample.BizFx.DevOps.Entities;
    using Plugin.Sample.JsonCommander.Commands;
    using Plugin.Sample.JsonCommander.Models;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionCleanEnvironment")]
    public class DoActionCleanEnvironment : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionCleanEnvironment(
            CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("DevOps-CleanEnvironment", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            var appService = await this._commerceCommander.GetEntity<AppService>(context.CommerceContext, entityView.ItemId);

            var jsonAction = new JsonAction { Environment = context.CommerceContext.Environment.Name };

            await this._commerceCommander.Command<JsonCommander>()
                .Put(context.CommerceContext, $"http://{appService.Host}/commerceops/CleanEnvironment()", jsonAction);

            return entityView;
        }
    }
}
