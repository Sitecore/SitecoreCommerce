using ShipEngine.ApiClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Sample.ShipEngine.Entities
{
    public class ShippingRateResult
    {
        public MoneyDTO ShippingRate { get; set; }

        public MoneyDTO InsuranceAmount { get; set; }
        public bool HasError { get { return !string.IsNullOrWhiteSpace(ErrorMessage); } }
        public bool IsErrorAddedToCommerceContext { get; set; }
        public string ErrorMessage { get; set; }
    }
}
