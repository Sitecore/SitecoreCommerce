namespace Plugin.Sample.Pricing.Generator.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Plugin.Sample.Pricing.Generator.Commands;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionClearPriceCards")]
    public class DoActionClearPriceCards : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionClearPriceCards(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("Pricing-ClearPriceCards", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                await this._commerceCommander.Command<ClearPriceCards>()
                    .Process(context.CommerceContext, entityView.ItemId);
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Pricing.DoActionGeneratorSamplePriceBook.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
