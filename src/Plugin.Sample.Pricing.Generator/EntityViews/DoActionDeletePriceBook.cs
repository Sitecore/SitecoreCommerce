namespace Plugin.Sample.Pricing.Generator.EntityViews
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Plugin.Sample.Pricing.Generator.Commands;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionDeletePriceBook")]
    public class DoActionDeletePriceBook : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionDeletePriceBook(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("Pricing-DeletePriceBook", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var bookName = entityView.ItemId.Replace("Entity-PriceBook-", "");

                await this._commerceCommander.Command<ClearPriceCards>()
                    .Process(context.CommerceContext, entityView.ItemId);

                await this._commerceCommander.DeleteEntity(context.CommerceContext, $"Entity-PriceBook-{bookName}");
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Pricing.DoActionDeletePriceBook.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
