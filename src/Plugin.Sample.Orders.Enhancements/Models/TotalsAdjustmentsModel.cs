
namespace Plugin.Sample.Orders.Enhancements.Models
{
    using Sitecore.Commerce.Core;

    public class TotalsAdjustmentsModel : Model
    {
        public TotalsAdjustmentsModel()
        {
            this.Adjustment = new Money();
        }

        public Money Adjustment { get; set; }
    }
}
