# Sitecore Commerce Sample Plugins
Contains sample code for Sitecore Commerce Plugins
These Plugins are targeted toward [Sitecore Experience Commerce 9.0 Update 3 release.](https://dev.sitecore.net/Downloads/Sitecore_Commerce/90/Sitecore_Experience_Commerce_90_Update3.aspx)

Clone this repository and add these Plugins as projects to your Customer.Sample.Solution.
Add the Reference to the Plugin in the Sitecore.Commerce.Engine project
Rebuild.  

[See the 9.0.3 documentation on deploying your custom XC solution](https://doc.sitecore.com/developers/90/sitecore-experience-commerce/en/deploying-your-sitecore-xc-solution.html).    

Included Plugins
+ BizFx.DevOps 
    + Provides a mechanism for listing Environments and viewing their details and viewing other metadata and configuration data of a Experienice Commerce deployment.
+ BizFx.Enhancements
    + Provides mechanisms for validating data in an Experience Commerce system.  Currently it can check to ensure Sellable Items (Products) have images and that Price Cards have snapshots.
+ Catalog.Generator 
    + Generates fake catalog data for testing or demo purposes.
+ ContentItemCommander 
    + A demostration of creating dashboards and other content items.  
+ Ebay 
    + Ebay Marketplace Integration from Experience Commerce and BizFx
+ Enhancements 
    + Plugin which enables other enhancements via the sample plugins. 
+ Entitlement.Enhancements 
    + Demostrates modifying the CustomerEntitlements view 
+ ExtendedConditions 
    + Sample conditions for the Experience Commerce Promotion Engine
+ HandleMissingSitecore
    + Enables Experience Commerce to function without a connection to Sitecore XP
+ JsonCommander 
    + A utility plugin used by other sample plugins.
+ ListMaster
    + Manages list of Commerce Entities
+ Orders.Enhancements 
    + Plugin which returns KPIs on Orders 
+ Pricing.Generator
    + Plugin for generating sample Price Cards 
+ Promotions.Generator 
    + Plugin for generating sample Promotions 
+ Search.Management 
    + A Plugin for managing search within Experience Commerce Engine 
+ VatTax
    + A Plugin for calculating simple VAT 

For more information please go to [Sitecore.net](http://www.sitecore.net)
