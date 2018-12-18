namespace Plugin.Sample.Promotions.Generator.EntityViews
{
    using System.Linq;
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("EnsureActions")]
    public class EnsureActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name == "PromotionsDashboard")
            {
                var promotionsEntityView = entityView.ChildViews.FirstOrDefault(p => p.Name == "PromotionBooks");

                promotionsEntityView?.GetPolicy<ActionsPolicy>().Actions.Add(new EntityActionView
                {
                    Name = "Promotions-GenerateSamplePromotionBook",
                    DisplayName = $"Generate Sample Promotion Book",
                    Description = string.Empty,
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "Promotions-GenerateSamplePromotionBook",
                    UiHint = string.Empty
                });
                return Task.FromResult(entityView);
            }

            return Task.FromResult(entityView);
        }
    }
}
