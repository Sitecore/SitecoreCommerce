
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.Composer.Tags
{
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.Views;
    using Sitecore.Commerce.Plugin.Catalog;

    /// <summary>
    /// Defines a block which validates, inserts and updates Actions for this Plugin.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("EnsureTaggedViews")]
    public class EnsureTaggedViews : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsureTaggedViews"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EnsureTaggedViews(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            if (entityView.EntityId.Contains("Entity-SellableItem-"))
            {
                var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);
                //var sellableItem = entityViewArgument.Entity as SellableItem;
                

                var tags = (entityViewArgument.Entity as SellableItem).Tags;

                var templates = await this._commerceCommander.Command<ListCommander>().GetListItems<ComposerTemplate>(context.CommerceContext, CommerceEntity.ListName<ComposerTemplate>(), 0, 100);

                foreach(var entityTemplate in templates)
                {
                    if (entityTemplate.Tags.Any(p=>tags.Any(q=>q.Name == p.Name)))
                    {
                        var templateName = entityTemplate.Name;

                        if (entityViewArgument.Entity.HasComponent<EntityViewComponent>())
                        {

                            var entityViewComponent = entityViewArgument.Entity.GetComponent<EntityViewComponent>();
                            if (entityViewComponent.View.ChildViews.Any(p => p.Name == templateName))
                            {
                                //Do Nothing
                            }
                            else
                            {
                                //Need to insert
                                var template = await this._commerceCommander.GetEntity<ComposerTemplate>(context.CommerceContext, $"{CommerceEntity.IdPrefix<ComposerTemplate>()}{templateName}");
                                var templateEntityViewComponent = template.GetComponent<EntityViewComponent>();

                                if (!templateEntityViewComponent.View.DisplayName.Contains("(Auto-Existing)"))
                                {
                                    templateEntityViewComponent.View.DisplayName = templateEntityViewComponent.View.DisplayName + "(Auto-Existing)";
                                }
                                entityViewComponent.View.ChildViews.Add(templateEntityViewComponent.View);
                            }
                        }
                        else
                        {
                            //No Component for Views, so we are going to add one
                            var entityViewComponent = entityViewArgument.Entity.GetComponent<EntityViewComponent>();
                            //Need to insert
                            var template = await this._commerceCommander.GetEntity<ComposerTemplate>(context.CommerceContext, $"{CommerceEntity.IdPrefix<ComposerTemplate>()}{templateName}");

                            var templateEntityViewComponent = template.GetComponent<EntityViewComponent>();

                            if (!templateEntityViewComponent.View.DisplayName.Contains("(Auto-NoExisting)"))
                            {
                                templateEntityViewComponent.View.DisplayName = templateEntityViewComponent.View.DisplayName + "(Auto-NoExisting)";
                            }

                            entityViewComponent.View.ChildViews.Add(templateEntityViewComponent.View);
                        }
                    }
                }
            }
            return entityView;
        }
    }
}
