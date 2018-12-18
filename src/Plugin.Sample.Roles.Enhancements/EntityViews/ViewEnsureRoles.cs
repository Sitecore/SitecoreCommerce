namespace Plugin.Sample.Roles.Enhancements.EntityViews
{
    using System.Threading.Tasks;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Pipelines;
    
    using PluginPolicy = global::Plugin.Sample.Roles.Enhancements.PluginPolicy;

    [PipelineDisplayName("ViewEnsureRoles")]
    public class ViewEnsureRoles : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (!string.IsNullOrEmpty(entityView.Action))
            {
                return Task.FromResult(entityView);
            }
            
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
                ItemId = string.Empty,
            };
            entityView.ChildViews.Add(contextView);
            
            var roles = context.CommerceContext.CurrentRoles();
            var rolesStr = string.Empty;
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
