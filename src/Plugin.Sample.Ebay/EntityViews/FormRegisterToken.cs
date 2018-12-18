
namespace Plugin.Sample.Ebay.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("FormRegisterToken")]
    public class FormRegisterToken : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "Ebay-FormRegisterToken")
            {
                return Task.FromResult(entityView);
            }
            
            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "EbayToken",
                    DisplayName = "Ebay User Token",
                    IsHidden = false,
                    IsReadOnly = false,
                    IsRequired = true,
                    UiType = string.Empty, 
                    RawValue = string.Empty
                });

            return Task.FromResult(entityView);
        }
    }

}
