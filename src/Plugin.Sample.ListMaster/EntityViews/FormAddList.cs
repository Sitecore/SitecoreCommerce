
namespace Plugin.Sample.ListMaster.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = global::Plugin.Sample.ListMaster.Policies.PluginPolicy;
    
    [PipelineDisplayName("FormAddList")]
    public class FormAddList : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "ListMaster-FormAddList")
            {
                return Task.FromResult(entityView);
            }
            
            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "List",
                    IsHidden = false,
                    IsRequired = false,
                    RawValue = string.Empty
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "AsManagedList",
                    IsHidden = false,
                    IsRequired = false,
                    RawValue = true
                });

            return Task.FromResult(entityView);
        }
    }

}
