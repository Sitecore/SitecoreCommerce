namespace Plugin.Sample.BizFx.DevOps.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionAddAppService")]
    public class DoActionAddAppService : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly IFindEntityPipeline _findEntityPipeline;

        public DoActionAddAppService( 
            IFindEntityPipeline findEntityPipeline)
        {
            this._findEntityPipeline = findEntityPipeline;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("DevOps-AddAppService", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            await this._findEntityPipeline.Run(new FindEntityArgument(typeof(CommerceEntity), entityView.ItemId, false), context.CommerceContext.GetPipelineContextOptions());
            
            return entityView;
        }
   }
}
