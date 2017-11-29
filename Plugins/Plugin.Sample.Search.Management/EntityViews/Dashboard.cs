
using System.Threading.Tasks;

namespace Plugin.Sample.Search.Management
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which populates an EntityView.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("Dashboard")]
    public class Dashboard : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {

        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dashboard"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public Dashboard(CommerceCommander commerceCommander)
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

            if (entityView.Name != "SearchDashboard")
            {
                return Task.FromResult(entityView);
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            entityView.UiHint = "Flat";
            entityView.Icon = pluginPolicy.Icon;
            entityView.DisplayName = "Search Dashboard";

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "ViewProperty1",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = "ValueString"
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "HtmlProperty",
                    IsHidden = false,
                    IsReadOnly = true,
                    OriginalType = "Html",
                    UiType = "Html",
                    RawValue = "<b>ValueString</b>"
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "ImageProperty",
                    IsHidden = false,
                    IsReadOnly = true,
                    OriginalType = "Html",
                    UiType = "Html",
                    RawValue = "<img alt='This is the alternate' height=100 width=100 src='http://scbizfx.westus.cloudapp.azure.com/-/media/Images/Habitat/6042177_01.ashx?h=625&w=770' style=''/>"
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "",
                    IsHidden = false,
                    IsReadOnly = true,
                    OriginalType = "Html",
                    UiType = "Html",
                    RawValue = "<img alt='This is the alternate' height=100 width=100 src='http://scbizfx.westus.cloudapp.azure.com/-/media/Images/Habitat/6042177_01.ashx?h=625&w=770' style=''/>"
                });
            
            var newEntityView = new EntityView {
                 Name = "Example Flat View",
                 UiHint = "Flat",
                 Icon = pluginPolicy.Icon,
                 ItemId = "",
            };

            newEntityView.Properties.Add(
                new ViewProperty { Name = "ViewProperty1",
                    IsHidden = false, IsReadOnly = true,
                    IsRequired = false, RawValue = "ValueString",
                    UiType = "EntityLink"
                });

            entityView.ChildViews.Add(newEntityView);

            return Task.FromResult(entityView);
        }
    }
}
