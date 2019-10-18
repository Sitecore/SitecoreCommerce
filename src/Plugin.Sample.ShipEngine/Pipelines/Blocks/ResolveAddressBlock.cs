using Plugin.Sample.ShipEngine.Policies;
using Sitecore.Commerce.Core;
using Sitecore.Framework.Conditions;
using Sitecore.Framework.Pipelines;
using System;
using System.Threading.Tasks;
using System.Linq;
using SEA = ShipEngine.ApiClient.Api;
using SEM = ShipEngine.ApiClient.Model;

namespace Plugin.Sample.ShipEngine.Pipelines.Blocks
{
    [PipelineDisplayName("Plugin.ShipEngine.Piplines.Blocks.ResolveAddressBlock")]
    public class ResolveAddressBlock : PipelineBlock<Party, Party, CommercePipelineExecutionContext>
    {
        public override async Task<Party> Run(Party arg, CommercePipelineExecutionContext context)
        {

            Condition.Requires(arg).IsNotNull($"{Name}: The unresolved party can not be null");

            var policy = context.CommerceContext.GetPolicy<ShipEnginePolicy>();

            if (policy.AddressValidationMethod == SEM.AddressValidatingShipment.ValidateAddressEnum.NoValidation)
            {
                return await Task.FromResult(arg);
            }

            //May be more conditions to check here.
            var ava = new SEA.AddressValidationApi();

            var newParty = arg;

            //Convert from Party to ShipEngine AddressDTO
            var request = new SEM.AddressDTO
            {
                AddressLine1 = newParty.Address1,
                AddressLine2 = String.IsNullOrWhiteSpace(newParty.Address2) ? string.Empty : newParty.Address2,
                CityLocality = newParty.City,
                StateProvince = String.IsNullOrWhiteSpace(newParty.State) ? newParty.StateCode : newParty.State,
                PostalCode = newParty.ZipPostalCode,
                CountryCode = newParty.CountryCode
            };

            var result = ava.AddressValidationValidateAddresses(
                new System.Collections.Generic.List<SEM.AddressDTO>() { request },
                policy.ApiKey
                );

            if (result != null && result.Count == 0)
            {
                return await Task.FromResult(arg);
            }

            var badAddress = result.Find(m => m.Status == SEM.AddressValidationResponseDTO.StatusEnum.Error);
            if (badAddress !=null)
            {
                var errorMessage = " - " +
                    string.Join(", ", from a in badAddress.Messages select a.Message);

                await context.CommerceContext.AddMessage(
                    context.GetPolicy<KnownResultCodes>().Error,
                    "InvalidClientPolicy",
                    new object[] { errorMessage }
                );

                return await Task.FromResult(arg);
            }

            if ( result.TrueForAll(a => a.Status == SEM.AddressValidationResponseDTO.StatusEnum.Verified))
            {

                if (policy.AddressValidationMethod == SEM.AddressValidatingShipment.ValidateAddressEnum.ValidateAndClean)
                {
                    
                    var res = result[0].MatchedAddress;

                    //Convert from AddressDTO to Party
                    newParty.Address1 = res.AddressLine1;
                    newParty.Address2 = res.AddressLine2;
                    newParty.City = res.CityLocality;
                    newParty.State = res.StateProvince;
                    newParty.ZipPostalCode = res.PostalCode;
                    newParty.CountryCode = res.CountryCode;
                }
            }

            return await Task.FromResult(newParty);
        }
    }
}
