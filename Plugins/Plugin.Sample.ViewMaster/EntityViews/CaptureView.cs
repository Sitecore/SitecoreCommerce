
using System.Threading.Tasks;

namespace Plugin.Sample.ViewMaster
{
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Core.Commands;
    using System.Text;
    using Newtonsoft.Json;
    using System.IO;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a block which populates an EntityView for a list of Coupons with the View named Public Coupons.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("CaptureView")]
    public class CaptureView : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureView"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public CaptureView(CommerceCommander commerceCommander)
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

            if (entityView.EntityId == "ViewMaster_Session")
            {
                //No Capture List
                return entityView;
            }

            //context.Logger.LogInformation($"======> Environment={context.CommerceContext.Environment.Name}|View={entityView.Name}|EntityId={entityView.EntityId}|Action={entityView.Action}|Shop={context.CommerceContext.CurrentShopName()}");

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            JsonSerializer Serializer = new JsonSerializer
            {
                TypeNameHandling = TypeNameHandling.All,
                NullValueHandling = NullValueHandling.Ignore
            };

            using (var writer = new JsonTextWriter(sw))
            {
                Serializer.Serialize(writer, entityView);
            }

            var entityViewAsJson = sw.ToString();

            if (entityView.Name == "ViewMaster")
            {
                //Avoid recording ViewMaster Views
                return entityView;
            }
            var sessionEntity = await this._commerceCommander.GetEntity<ViewmasterSessionEntity>(context.CommerceContext, "ViewMaster_Session", true);
            if (!sessionEntity.IsPersisted)
            {
                sessionEntity.Id = "ViewMaster_Session";
            }
            //var entityViewAsJson = await this._commerceCommander.Command<EntitySerializerCommand>().SerializeEntity(context.CommerceContext, entityView);
            //context.Logger.LogInformation(entityViewAsJson);

            var viewRecord = new ViewMasterViewRecord();
            viewRecord.View = entityView;
            foreach(var header in context.CommerceContext.Headers)
            {
                viewRecord.Headers.Add(header.Key, header.Value);
            }
            sessionEntity.Views.Add(viewRecord);

            //var persistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, sessionEntity);

            return entityView;
        }
    }
}
