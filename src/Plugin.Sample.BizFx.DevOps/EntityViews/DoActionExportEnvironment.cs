namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = Plugin.Sample.BizFx.DevOps.Policies.PluginPolicy;

    [PipelineDisplayName("DoActionExportEnvironment")]
    public class DoActionExportEnvironment : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        private static readonly JsonSerializerSettings Serializer = new JsonSerializerSettings
        {  
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore
        };

        public DoActionExportEnvironment(
            CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("DevOps-ExportEnvironment", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var name = entityView.ItemId.Replace("Entity-CommerceEnvironment-", "");

                await this._commerceCommander.Command<GetEnvironmentCommand>()
                    .Process(this._commerceCommander.GetGlobalContext(context.CommerceContext), name);
                
                var exportEnvironmentResult = await this._commerceCommander.Command<ExportEnvironmentCommand>()
                    .Process(context.CommerceContext, name);

                var path = this._commerceCommander.CurrentNodeContext(context.CommerceContext).WebRootPath + @"\data\environments";

                File.WriteAllText(path + $@"\" + name + ".json", exportEnvironmentResult);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"DevOps.DoActionPullEnvironment.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
