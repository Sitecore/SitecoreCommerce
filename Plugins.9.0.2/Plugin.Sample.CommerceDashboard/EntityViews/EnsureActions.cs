
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.CommerceDashboard
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
    [PipelineDisplayName("EnsureActions")]
    public class EnsureActions : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="EnsureActions"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public EnsureActions(CommerceCommander commerceCommander)
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

            if (entityView.Name != "MyDashboard")
            {
                return Task.FromResult(entityView);
            }

            var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();

            var tableEntityView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Example Table View");

            if (tableEntityView != null)
            {
                var tableEntityViewActionsPolicy = tableEntityView.GetPolicy<ActionsPolicy>();

                //This action adds a SampleDashboardEntity
                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "MyDashboard-AddDashboardEntity",
                    DisplayName = "Adds a new Sample Dashboard Entity",
                    Description = "Adds a new Sample Dashboard Entity",
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = "MyDashboard-FormAddDashboardEntity",
                    Icon = "add"
                });

                //This action removes a SampleDashboardEntity
                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "MyDashboard-RemoveDashboardEntity",
                    DisplayName = "Removes a Sample Dashboard Entity",
                    Description = "Removes a Sample Dashboard Entity",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "",
                    Icon = "delete"
                });

            }

            return Task.FromResult(entityView);
        }
    }
}
