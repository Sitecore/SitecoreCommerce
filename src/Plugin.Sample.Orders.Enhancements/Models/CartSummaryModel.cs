
namespace Plugin.Sample.Orders.Enhancements.Models
{
    using Sitecore.Commerce.Core;

    public class CartSummaryModel : Model
    {
        public CartSummaryModel()
        {
            this.CartId = string.Empty;
            this.Status = string.Empty;
        }
        
        public string CartId { get; set; }
        
        public string Status { get; set; }
    }
}
