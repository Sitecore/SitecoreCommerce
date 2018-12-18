
namespace Plugin.Sample.ViewMaster.EntityViews
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using global::Plugin.Sample.ViewMaster.Entities;

    using Newtonsoft.Json;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
 
    [PipelineDisplayName("CaptureView")]
    public class CaptureView : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public CaptureView(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");
            
            if (entityView.EntityId == "ViewMaster_Session")
            {
                return entityView;
            }
            
            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            JsonSerializer serializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.All,
                NullValueHandling = NullValueHandling.Ignore
            };

            using (var writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, entityView);
            }
            
            if (entityView.Name == "ViewMaster")
            {
                return entityView;
            }
            var sessionEntity = await this._commerceCommander.GetEntity<ViewMasterSessionEntity>(context.CommerceContext, "ViewMaster_Session", true);
            if (!sessionEntity.IsPersisted)
            {
                sessionEntity.Id = "ViewMaster_Session";
            }

            var viewRecord = new ViewMasterViewRecord { View = entityView };
            foreach(var header in context.CommerceContext.Headers)
            {
                viewRecord.Headers.Add(header.Key, header.Value);
            }

            sessionEntity.Views.Add(viewRecord);

            return entityView;
        }
    }
}
