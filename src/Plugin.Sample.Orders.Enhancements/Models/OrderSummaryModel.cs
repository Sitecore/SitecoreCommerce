namespace Plugin.Sample.Orders.Enhancements.Models
{
    using Sitecore.Commerce.Core;

    public class OrderSummaryModel : Model
    {
        public OrderSummaryModel()
        {
            this.OrderId = string.Empty;
            this.ConfirmationId = string.Empty;
            this.Status = string.Empty;
        }

        public string OrderId { get; set; }

        public string ConfirmationId { get; set; }

        public string Status { get; set; }
    }
}
