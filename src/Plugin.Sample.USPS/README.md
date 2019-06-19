# Plugin.Sample.USPS
This is a simple plugin to resolve United States addresses against the US Postal Service validate address service.

## System Requirements
* Sitecore Experience Commerce 9.1
* Free USPS Web Tools Account - [Register Here](https://www.usps.com/business/web-tools-apis/web-tools-registration.htm)

## Installation & Configuration
1. Clone the repository and build
2. Set the UserId in *Config\Plugin.USPS.PolicySet-1.0.0.json* to the ID given to you during the USPS Web Tools registration process
3. Copy **Plugin.Sample.USPS.dll** to the *wwwroot* folder of the Authoring and Shops instances of the Commerce Engine
4. Copy  **Config\Plugin.USPS.PolicySet-1.0.0.json** to the *wwwroot\data\Environments* of the same instances from above
5. Depending on your configuration, you'll need to add the following snippet to the Authoring and Shops policy for your Environment (e.g. *PlugIn.Habitat.CommerceShops-1.0.0.json*)
```
{
	"$type": "Sitecore.Commerce.Core.PolicySetPolicy, Sitecore.Commerce.Core",
	"PolicySetId": "Entity-PolicySet-USPSPolicySet"
} 
```
6. Use Postman to re-Boostrap the Commerce Engine. **NOTE: This only needs to be done on one instance and will apply to all of them**

## Operation
The ResolveAddress block is added to the *IValidateParty* pipeline and will automatically fire once an address is submitted through Account Management or Checkout. If the address can be resolved any spelling corrections, abbreviation standardizations, and Zip Code expansion/corrections will be made before saving to the database. If it can't then the original input address will stay unchanged. Any errors with the plugin will be written to the standard engine log.

## Additonal Information
* Technical information about the Address Validation API can be found [here](https://www.usps.com/business/web-tools-apis/address-information-api.htm)
