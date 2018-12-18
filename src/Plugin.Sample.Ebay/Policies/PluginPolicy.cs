
namespace Plugin.Sample.Ebay.Policies
{
    using Sitecore.Commerce.Core;
    
    public class PluginPolicy : Policy
    {
        public PluginPolicy()
        {
            this.Icon = "market_stand";
            this.IsDisabled = true;
        }
        
        public string Icon { get; set; }
        
        public bool IsDisabled { get; set; }
    }
}
