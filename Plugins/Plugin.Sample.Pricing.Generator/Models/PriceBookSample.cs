
using System;

namespace Plugin.Sample.Pricing.Generator
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;
  
    /// <summary>
    /// Defines a PriceBookSample
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.Model" />
    public class PriceBookSample : Model
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PriceBookSample"/> class.
        /// </summary>
        public PriceBookSample()
        {
            this.Id = string.Empty;
            this.DisplayText = string.Empty;
        }

        /// <summary>
        /// Private Coupon Prefix
        /// </summary>
        public string PrivateCouponPrefix { get; set; }

        /// <summary>
        /// Private Coupon Suffix
        /// </summary>
        public string PrivateCouponSuffix { get; set; }

        /// <summary>
        /// Public Coupon Code
        /// </summary>
        public string PublicCouponCode { get; set; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the book.
        /// </summary>
        /// <value>
        /// The book.
        /// </value>
        public EntityReference Book { get; set; }

        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        /// <value>
        /// The display text.
        /// </value>
        public string DisplayText { get; set; }


        /// <summary>
        /// Determines whether the specified context is approved.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if the specified context is approved; otherwise, <c>false</c>.
        /// </returns>
        public bool IsApproved(CommerceContext context)
        {
            Condition.Requires(context, nameof(context)).IsNotNull();

            //var isApproved = this.GetComponent<ApprovalComponent>()
            //        .Status.Equals(context.GetPolicy<ApprovalStatusPolicy>().Approved, StringComparison.OrdinalIgnoreCase);
            //var isDisabled = this.HasPolicy<DisabledPolicy>() && this.DateUpdated.HasValue && this.DateUpdated.Value.CompareTo(context.CurrentEffectiveDate()) <= 0;

            //return isApproved && !isDisabled;
            return true;
        }

        /// <summary>
        /// Determines whether the specified context is draft.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///   <c>true</c> if the specified context is draft; otherwise, <c>false</c>.
        /// </returns>
        public bool IsDraft(CommerceContext context)
        {
            Condition.Requires(context, nameof(context)).IsNotNull();

            //return this.GetComponent<ApprovalComponent>().Status.Equals(context.GetPolicy<ApprovalStatusPolicy>().Draft, StringComparison.OrdinalIgnoreCase);
            return true;
        }



    }
}
