
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
    [PipelineDisplayName("EnsureTemplateTags")]
    public class EnsureTemplateTags : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsureTemplateTags"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EnsureTemplateTags(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            if (entityView.Name == "ComposerDashboard")
            {
                var composerTemplatesView = entityView.ChildViews.FirstOrDefault(p => p.Name == "ComposerTemplates");

                if (composerTemplatesView != null)
                {
                    foreach(var childView in (composerTemplatesView as EntityView).ChildViews)
                    {
                        var itemId = (childView as EntityView).Properties.FirstOrDefault(p => p.Name == "ItemId").RawValue as string;
                        var foundEntity = context.CommerceContext.GetObjects<FoundEntity>().FirstOrDefault(p => p.EntityId == itemId);
                        if (foundEntity != null)
                        {
                            var tags = "";
                            var composerTemplate = foundEntity.Entity as ComposerTemplate;

                            foreach (var tag in composerTemplate.Tags)
                            {
                                if (string.IsNullOrEmpty(tags))
                                {
                                    tags = tags + tag.Name;
                                }
                                else
                                {
                                    tags = tags + "|" + tag.Name;
                                }
                            }
                            (childView as EntityView).Properties.Add(new ViewProperty { Name = "Tags", DisplayName = "Tags", RawValue = tags });
                        }
                    }
                }

                return Task.FromResult(entityView);
            }



            return Task.FromResult(entityView);
        }
    }
}
