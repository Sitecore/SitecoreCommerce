
namespace Plugin.Sample.ListMaster.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("FormImportList")]
    public class FormImportList : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "ListMaster_ImportList")
            {
                return Task.FromResult(entityView);
            }
            
            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Path",
                    IsHidden = false,
                    IsReadOnly = false,
                    IsRequired = true,
                    RawValue = @"C:\Users\kha\Documents\ExportedEntities"
                });

            return Task.FromResult(entityView);
        }
    }
}
