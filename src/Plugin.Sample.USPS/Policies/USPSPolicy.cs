using Sitecore.Commerce.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.Sample.USPS.Policies
{
    public class USPSPolicy : Policy
    {
        public string UserId { get; set; }
        public string Url { get; set; }
    }
}
