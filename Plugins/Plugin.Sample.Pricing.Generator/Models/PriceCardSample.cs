
using System;

namespace Plugin.Sample.Pricing.Generator
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Conditions;
 
    /// <summary>
    /// Defines a PromotionSample
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.Model" />
    public class PriceCardSample : Model
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PriceCardSample"/> class.
        /// </summary>
        public PriceCardSample()
        {
            this.Id = string.Empty;
            //this.Book = new EntityReference { EntityTarget = bookId };
            this.Description = string.Empty;
            this.DisplayName = string.Empty;
            this.IndexId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; private set; }

        /// <summary>
        /// Gets or sets the index identifier.
        /// </summary>
        /// <value>
        /// The index identifier.
        /// </value>
        public string IndexId { get; set; }

        /// <summary>
        /// Gets or sets the book.
        /// </summary>
        /// <value>
        /// The book.
        /// </value>
        public EntityReference Book { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>
        /// The display name.
        /// </value>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

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
