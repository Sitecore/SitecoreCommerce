
using System.Threading.Tasks;

namespace Plugin.Sample.VatTax
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    /// <summary>
    /// Defines a block which populates an EntityView for a list of Coupons with the View named Public Coupons.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("FormAddDashboardEntity")]
    public class FormAddDashboardEntity : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormAddDashboardEntity"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public FormAddDashboardEntity(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>Runs the Command.</summary>
        /// <param name="entityView">The argument.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="EntityView"/>.</returns>
        public override Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            Condition.Requires(entityView).IsNotNull($"{this.Name}: The argument cannot be null");

            if (entityView.Name != "VatTaxDashboard-FormAddDashboardEntity")
            {
                return Task.FromResult(entityView);
            }

            var entityViewArgument = this._commerceCommander.Command<ViewCommander>().CurrentEntityViewArgument(context.CommerceContext);

            var pluginPolicy = context.GetPolicy<PluginPolicy>();


            //entityView.Properties.Add(
            //    new ViewProperty
            //    {
            //        Name = "Name",
            //        IsHidden = false,
            //        //IsReadOnly = true,
            //        IsRequired = false,
            //        RawValue = ""
            //    });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "TaxTag",
                    IsHidden = false,
                    //IsReadOnly = true,
                    IsRequired = true,
                    RawValue = ""
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "CountryCode",
                    IsHidden = false,
                    //IsReadOnly = true,
                    IsRequired = true,
                    RawValue = ""
                });

            entityView.Properties.Add(
                new ViewProperty
                {
                    Name = "TaxPct",
                    IsHidden = false,
                    //IsReadOnly = true,
                    IsRequired = true,
                    RawValue = 0
                });

            return Task.FromResult(entityView);
        }


    }

}
