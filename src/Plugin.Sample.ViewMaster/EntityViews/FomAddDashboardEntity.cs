
namespace Plugin.Sample.ViewMaster.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using PluginPolicy = global::Plugin.Sample.ViewMaster.Policies.PluginPolicy;
    
    [PipelineDisplayName("FomAddDashboardEntity")]
    public class FomAddDashboardEntity : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        public FomAddDashboardEntity(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "MyDashboard-FormAddDashboardEntity")
            {
                return Task.FromResult(entityView);
            }
            
            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "Name",
                    IsHidden = false,
                    IsRequired = false,
                    RawValue = string.Empty
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "DisplayName",
                    IsHidden = false,
                    IsRequired = false,
                    RawValue = string.Empty,
                    UiType = "RichText"
                });

            return Task.FromResult(entityView);
        }
    }
}
