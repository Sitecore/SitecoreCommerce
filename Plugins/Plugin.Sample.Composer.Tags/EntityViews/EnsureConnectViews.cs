
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
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Defines a block which validates, inserts and updates Actions for this Plugin.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("EnsureConnectViews")]
    public class EnsureConnectViews : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsureConnectViews"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EnsureConnectViews(CommerceCommander commerceCommander)
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

            if (entityView.Name.Contains("ConnectSellableItem"))
            {
                var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);
                //var sellableItem = entityViewArgument.Entity as SellableItem;


                //var tags = (entityViewArgument.Entity as SellableItem).Tags;

                var templates = await this._commerceCommander.Command<ListCommander>().GetListItems<ComposerTemplate>(context.CommerceContext, CommerceEntity.ListName<ComposerTemplate>(), 0, 100);

                foreach (var entityTemplate in templates)
                {

                    var templateName = entityTemplate.Name;

                    var entityViewComponents = entityTemplate.Components.OfType<EntityViewComponent>();
                    foreach (var entityViewComponent in entityViewComponents)
                    {
                        var displayNameProperty = entityViewComponent.View.Properties.FirstOrDefault(p => p.Name == "Name");
                        if (displayNameProperty != null)
                        {
                            entityViewComponent.View.Properties.Remove(displayNameProperty);
                        }

                        var nameProperty = entityViewComponent.View.Properties.FirstOrDefault(p => p.Name == "Name");
                        if (nameProperty != null)
                        {
                            entityViewComponent.View.Properties.Remove(nameProperty);
                        }
                        
                        entityView.ChildViews.Add(entityViewComponent.View);
                    }



                }
            }
            return entityView;
        }
    }
}
