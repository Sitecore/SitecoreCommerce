using Sitecore.Commerce.Core;
using Plugin.Sample.USPS.Entities;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Plugin.Sample.USPS.Pipelines.Blocks
{
    [PipelineDisplayName("ResolveAddressBlock")]
    public class ResolveAddressBlock : PipelineBlock<Party, Party, CommercePipelineExecutionContext>
    {         
        public override Task<Party> Run(Party arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{Name}: The unresolved party can not be null");
            //May be more conditions to check here.

            var policy = context.CommerceContext.GetPolicy<Policies.USPSPolicy>();
            var newParty = arg;            

            if (newParty.CountryCode.ToUpperInvariant() != "US") { return Task.FromResult(newParty); }

            //Convert from Party to USPSRequest
            var request = new USPSAddressValidationRequest() { UserId = policy.UserId }; 

            request.Address.Address1 = newParty.Address1;
            request.Address.Address2 = String.IsNullOrWhiteSpace(newParty.Address2) ? string.Empty : newParty.Address2;
            request.Address.City = newParty.City;
            request.Address.State = String.IsNullOrWhiteSpace(newParty.State) ? newParty.StateCode : newParty.State;
            request.Address.Zip5 = newParty.ZipPostalCode.Substring(0, 5);

            var result = USPSValidate(request, policy);

            if (result != null)
            {
                //Convert from USPSResponse to Party
                newParty.Address1 = String.IsNullOrWhiteSpace(result.Address.Address1) && !String.IsNullOrWhiteSpace(result.Address.Address2) ? result.Address.Address2 : result.Address.Address1;
                newParty.Address2 = String.IsNullOrWhiteSpace(result.Address.Address1) ? string.Empty : result.Address.Address2;
                newParty.City = result.Address.City;
                newParty.State = result.Address.State;
                newParty.StateCode = result.Address.State;
                newParty.ZipPostalCode = (String.IsNullOrEmpty(result.Address.Zip4) ? result.Address.Zip5 : $"{result.Address.Zip5}-{result.Address.Zip4}");
            }

            return Task.FromResult(newParty);
        }

        private static USPSAddressValidationResponse USPSValidate(USPSAddressValidationRequest request, Policies.USPSPolicy policy)
        {
            var s = System.Xml.Serialization.XmlSerializer.FromTypes(new Type[] { typeof(USPSAddressValidationRequest), typeof(USPSAddressValidationResponse) });
            var sb = new System.Text.StringBuilder();

            using (var w = new System.IO.StringWriter(sb))
            {
                s[0].Serialize(w, request);
                w.Flush();
            }

            String rawResponse = string.Empty;
            var url = new Uri($"{policy.Url}{sb.ToString()}"); //TODO: Externalize
            using (var client = new HttpClient())
            {
                rawResponse = client.GetStringAsync(url).Result;
            }

            if (rawResponse.Contains("<Error>"))
            {
                var rdr = XElement.Parse(rawResponse);
                var errMsg = (from item in rdr.Descendants("Description")
                              select item.Value).Single();

                return null;
            }
            else
            {                
                USPSAddressValidationResponse resp = null;
                using (var r = XmlReader.Create(new StringReader(rawResponse)))
                {
                    resp = s[1].Deserialize(r) as USPSAddressValidationResponse;
                }

                return resp;
            }
        }
    }
}
