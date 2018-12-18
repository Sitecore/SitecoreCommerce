namespace Plugin.Sample.Ebay.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using global::Plugin.Sample.Ebay.Commands;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionPublishPending")]
    public class DoActionPublishPending : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public DoActionPublishPending(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("Ebay-PublishPending", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(entityView);
            }

            Task.Factory.StartNew(() => this._commerceCommander.Command<EbayCommand>().PublishPending(context.CommerceContext));

            return Task.FromResult(entityView);
        }
    }
}
