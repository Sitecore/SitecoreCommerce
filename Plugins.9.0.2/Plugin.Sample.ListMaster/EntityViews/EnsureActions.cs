
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.ListMaster
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

            if (entityView.Name != "ListMaster")
            {
                return Task.FromResult(entityView);
            }

            var actionsPolicy = entityView.GetPolicy<ActionsPolicy>();

            //var tableEntityView = entityView.ChildViews.FirstOrDefault(p => p.Name == "Views Recorded");

            if (entityView != null)
            {
                var tableEntityViewActionsPolicy = entityView.GetPolicy<ActionsPolicy>();

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "ListMaster-AddList",
                    DisplayName = "Add A List",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = "ListMaster-FormAddList",
                    Icon = "add"
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "ListMaster-DeleteList",
                    DisplayName = "Delete A List",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = true,
                    EntityView = "",
                    Icon = "delete"
                });

                //tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                //{
                //    Name = "ListMaster-ClearLists",
                //    DisplayName = "Clear All Lists",
                //    Description = "",
                //    IsEnabled = true,
                //    RequiresConfirmation = true,
                //    EntityView = "",
                //    Icon = "delete"
                //});

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "ListMaster-ExportList",
                    DisplayName = "Publish List",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = "ListMaster_ExportList",
                    Icon = "escalator_up"
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "ListMaster-ImportList",
                    DisplayName = "Import List",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = "ListMaster_ImportList",
                    Icon = "escalator_down"
                });

                tableEntityViewActionsPolicy.Actions.Add(new EntityActionView
                {
                    Name = "ListMaster-SetPublishPath",
                    DisplayName = "Set Publish Path",
                    Description = "",
                    IsEnabled = true,
                    RequiresConfirmation = false,
                    EntityView = "ListMaster_SetPublishPath",
                    Icon = "escalator_down"
                });

            }

            return Task.FromResult(entityView);
        }
    }
}
