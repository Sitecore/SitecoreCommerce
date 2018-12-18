namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using Plugin.Sample.BizFx.DevOps.Entities;
    using Plugin.Sample.JsonCommander.Commands;
    using Plugin.Sample.JsonCommander.Components;
    using Plugin.Sample.JsonCommander.Models;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionInitializeEnvironment")]
    public class DoActionInitializeEnvironment : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionInitializeEnvironment(
            CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("DevOps-InitializeEnvironment", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            var appService = await this._commerceCommander.GetEntity<AppService>(context.CommerceContext, entityView.ItemId);

            try
            {
                var jsonResponse = await this._commerceCommander.Command<JsonCommander>()
                    .Process(context.CommerceContext, $"http://{appService.Host}/commerceops/InitializeEnvironment(environment='{context.CommerceContext.Environment.Name}')");

                appService.GetComponent<ActionHistoryComponent>()
                    .AddHistory(new ActionHistoryModel
                    {
                        Name = entityView.Action,
                        Response = "Ok",
                        JSON = jsonResponse.Json,
                        EntityId = entityView.EntityId,
                        ItemId = entityView.ItemId
                    });
                
                await this._commerceCommander.PersistEntity(context.CommerceContext, appService);
            }
            catch(Exception ex)
            {
                appService.GetComponent<ActionHistoryComponent>()
                    .AddHistory(new ActionHistoryModel
                    {
                        Name = entityView.Action,
                        Response = "Exception",
                        JSON = ex.StackTrace,
                        EntityId = entityView.EntityId,
                        ItemId = entityView.ItemId
                    });
            }

            return entityView;
        }
    }
}
