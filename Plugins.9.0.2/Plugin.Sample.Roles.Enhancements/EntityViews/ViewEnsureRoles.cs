
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Plugin.Sample.Roles.Enhancements
{
    
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;
    

    /// <summary>
    /// Defines a block which populates an EntityView for a Sample Page in the Sitecore Commerce Focused Commerce Experience.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("ViewEnsureRoles")]
    public class ViewEnsureRoles : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {

        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewEnsureRoles"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public ViewEnsureRoles(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>The execute.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {

            if (!string.IsNullOrEmpty(entityView.Action))
            {
                return Task.FromResult(entityView);
            }

            //Cant show in Table mode yet
            if (entityView.UiHint == "Table")
            {
                return Task.FromResult(entityView);
            }

            var pluginPolicy = context.GetPolicy<PluginPolicy>();

            if (pluginPolicy.IsDisabled)
            {
                return Task.FromResult(entityView);
            }

            var contextView = new EntityView
            {
                Name = "Context",
                DisplayName = "Call Context",
                UiHint = "Flat",
                DisplayRank = 50,
                Icon = pluginPolicy.Icon,
                ItemId = "",
            };
            entityView.ChildViews.Add(contextView);


            var roles = context.CommerceContext.CurrentRoles();
            var rolesStr = "";
            foreach(var role in roles)
            {
                if (string.IsNullOrEmpty(rolesStr))
                {
                    rolesStr = rolesStr + role;
                }
                else
                {
                    rolesStr = rolesStr + ", " + role;
                }
            }
            //entityView.DisplayName = entityView.DisplayName + $"({rolesStr})";
            contextView.Properties.Add(
                new ViewProperty
                {
                    Name = "Roles",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = rolesStr
                });


            contextView.Properties.Add(
                new ViewProperty
                {
                    Name = "CsrEmail",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = context.CommerceContext.CurrentCsrEmail()
                });

            contextView.Properties.Add(
                new ViewProperty
                {
                    Name = "CsrId",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = context.CommerceContext.CurrentCsrId()
                });

            contextView.Properties.Add(
                new ViewProperty
                {
                    Name = "currency",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = context.CommerceContext.CurrentCurrency()
                });

            contextView.Properties.Add(
                new ViewProperty
                {
                    Name = "CustomerId",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = context.CommerceContext.CurrentCustomerId()
                });

            contextView.Properties.Add(
                new ViewProperty
                {
                    Name = "EffectiveDate",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = context.CommerceContext.CurrentEffectiveDate()
                });

            contextView.Properties.Add(
                new ViewProperty
                {
                    Name = "Lat",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = context.CommerceContext.CurrentGeoLocation().Latitude
                });

            contextView.Properties.Add(
                new ViewProperty
                {
                    Name = "Ip",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = context.CommerceContext.CurrentIpAddress()
                });

            contextView.Properties.Add(
                new ViewProperty
                {
                    Name = "Language",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = context.CommerceContext.CurrentLanguage()
                });

            contextView.Properties.Add(
                new ViewProperty
                {
                    Name = "Shop",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = context.CommerceContext.CurrentShopName()
                });

            contextView.Properties.Add(
                new ViewProperty
                {
                    Name = "ShopperId",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = context.CommerceContext.CurrentShopperId()
                });

            contextView.Properties.Add(
                new ViewProperty
                {
                    Name = "Ip",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = context.CommerceContext.CurrentLanguage()
                });

            contextView.Properties.Add(
                new ViewProperty
                {
                    Name = "IsRegistered",
                    IsHidden = false,
                    IsReadOnly = true,
                    RawValue = context.CommerceContext.CurrentUserIsRegistered()
                });

            return Task.FromResult(entityView);
        }
    }
}
