
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.Orders.Enhancements
{
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;

    /// <summary>
    /// Defines a block which validates, inserts and updates Actions for this Plugin.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("ViewOrderMoveLinesUp")]
    public class ViewOrderMoveLinesUp : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewOrderMoveLinesUp"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public ViewOrderMoveLinesUp(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null) return Task.FromResult(entityView);

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            if (!entityView.EntityId.Contains("Entity-Order-"))
            {
                return Task.FromResult(entityView);
            }

            if (entityView.Name != "Master")
            {
                return Task.FromResult(entityView);
            }

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            //Clean up top level useless properties
            while (entityView.Properties.Count > 0)
            {
                entityView.Properties.Remove(entityView.Properties.First());
            }

            var linesView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Lines");
            if (linesView != null)
            {
                (linesView as EntityView).DisplayRank = 100;
            }

            var adjustmentsView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Adjustments");
            if (adjustmentsView != null)
            {
                (adjustmentsView as EntityView).DisplayRank = 150;
            }


            return Task.FromResult(entityView);
        }
    }
}
