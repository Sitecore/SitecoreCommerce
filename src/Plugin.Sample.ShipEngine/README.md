# Plugin.Sitecore.ShipEngine

## Introduction
This plugin integrates with ShipEngine to provide fulfillment option for Sitecore eXerience Commerce (XC) to multiple carriers, which include: FedEx, USPS, UPS, DHL, Imex, Dpd, etc. For listed of supported carriers see [ShipEngine Integrations](https://www.shipengine.com/integrations/)

## Prerequisites
Shipping costs depends on product size and weight, which are not specified in sample SXA storefront.
These items must be specified by navigating to Sitecore eXperience Commerce (XC) Business Tools and entering respective values for catalog items under Item Specification.
### 1. Allowed Units
Catalog items must containt both dimension and weight.  Below are the accepted unit of measurement for each:
* **Dimension units**: `Inch` or `Centemiter`
* **Weight units**: `Ounce`, `Pound`, `Gram`, or `Kilogram`

### 2. Carrier Options
In [ShipEngine application](https://app10.shipengine.com/) you must add your prefered carrier (e.g. DHL, FedEx, UPS, USPS, etc) and specify the prefered carrier by setting the **Account Nickname** to `Default`.
During this setup, you specify the mehthod in which the fulfillment will be carried out (e.g. Daily Pickup, Occasional Pickup, Customer Counter, etc).

### 3. Specify Fullfilment Methods
Though each carrier provides several fullfilment options, you may decide some options are not appropriate for your business (e.g. international).
See Sitecore [Walkthrough: Configuring fulfillment options](https://doc.sitecore.com/developers/90/sitecore-experience-commerce/en/walkthrough--configuring-fulfillment-options.html) to add your fulfillment options for each carrier.
For example, to support UPS Ground, you will need to create **UPS Ground** Sitecore item to `/sitecore/Commerce/Commerce Control Panel/Shared Settings/Fulfillment Options/Ship items/UPS Ground` as well as corresponding Commerce Term `/sitecore/Commerce/Commerce Control Panel/Storefront Settings/Commerce Terms/Shipping/UPS Ground`. 
Remember to [synronize content items](https://doc.sitecore.com/developers/91/sitecore-experience-commerce/en/synchronize-content-items.html) for the fulfillment options being added.

Note that this plugin is completely seperated from commerce storefront; therefore, the corresponding fullfilment carrier options must be spefied in the plugin policy file `data\Environments\Plugin.ShipEngine.PolicySet-1.0.0.json` for example: 
```json
      ...
  "Policies": {
    "$type": "System.Collections.Generic.List`1[[Sitecore.Commerce.Core.Policy, Sitecore.Commerce.Core]], mscorlib",
    "$values": [
      {
        "$type": "Plugin.Sample.ShipEngine.Policies.ShipEnginePolicy, Plugin.Sample.ShipEngine",
        "ApiKey": "SHIP_ENGINE_API_KEY_HERE", /* API Key provided by ShipEngine */
        ...
        "ShippingMethods": {
          "$type": "System.Collections.Generic.List`1[[System.String, mscorlib]], mscorlib",
          "$values": [                        /* See: ShipEngine Supported Carriers_Order Sources (w_ API codes).xlsx */
            "UPS Ground|ups_ground",
            "UPS Express®|ups_express"
          ]
      ...
```

### 4. Add Visual Studio Project
Add the visual studio projcect to your solution and load the *Plugin.ShipEngine.PolicySet-1.0.0.json* policy in your environment by specifying the following
```json
      ...
    "Policies":  {
        "$type":  "System.Collections.Generic.List`1[[Sitecore.Commerce.Core.Policy, Sitecore.Commerce.Core]], mscorlib",
        "$values":  [
              ...
              {
                "$type": "Sitecore.Commerce.Core.PolicySetPolicy, Sitecore.Commerce.Core",
                "PolicySetId": "Entity-PolicySet-ShipEnginePolicySet"
              },
              ...
```
For example, as part of your deployment you may update `C:\inetpub\wwwroot\CommerceAuthoring_Sc91\wwwroot\data\Environments\PlugIn.Habitat.CommerceAuthoring-1.0.0.json` environement with the above code snippet.


Remember to [Bootstrap and  InitializeEnvironment](https://doc.sitecore.com/developers/92/sitecore-experience-commerce/en/commerceops-api-actions.html) via [Postman](https://doc.sitecore.com/developers/92/sitecore-experience-commerce/en/postman-samples.html) to load your new Sitecore policies.

## Limitation
* ShipEngine can accomidate multiple carriers; but, this version supports one carrier at this time. 
* ShipEngine supports the ability to get best fulfillment price from multiple carriers; however, this option has not been implemented due to a single carrier implementation.
* ShipEngine supports printing mailing lables; but, this feature has not been implemented.
* ShipEngine supports address validation, which has been added to Sitecore pipeline.  However, this also means that there is a seprate charge for validation than for price quote. Contingent on your business needs, you can use address validation during checkout and remove it from Sitecore pipeline by commenting out the following expression in `ConfigureSitecore.cs`
```json
      ...
       /*
          .ConfigurePipeline<IValidatePartyPipeline>(d =>
          {
          d.Add<ResolveAddressBlock>().After<ValidatePartyBlock>();
          })
       */
      ...
```
* Sitecore SXA storefront renderings issue multiple calls to the plugin, which will result multiple ShipEngine API calls.  As a result cost of cost per API call.
