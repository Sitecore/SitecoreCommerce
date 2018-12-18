
namespace Plugin.Sample.Ebay.EntityViews
{
    using System.Threading.Tasks;

    using global::Plugin.Sample.Ebay.Components;
    using global::Plugin.Sample.Ebay.Entities;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Inventory;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("FormConfigure")]
    public class FormConfigure : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public FormConfigure(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }
        
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "Ebay-FormConfigure")
            {
                return entityView;
            }
            
            var ebayConfig = await this._commerceCommander.GetEntity<EbayConfigEntity>(context.CommerceContext, "Entity-EbayConfigEntity-Global", true);

            var ebayGlobalConfigComponent = ebayConfig.GetComponent<EbayGlobalConfigComponent>();
            
            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "ReturnsPolicy",
                    DisplayName = "Returns Policy",
                    IsHidden = false,
                    IsRequired = true,
                    RawValue = ebayGlobalConfigComponent.ReturnsPolicy
                });

            await this._commerceCommander.Command<GetInventorySetsCommand>().Process(context.CommerceContext);
            
            return entityView;
        }
    }
}
