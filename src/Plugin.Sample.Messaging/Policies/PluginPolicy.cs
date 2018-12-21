
namespace Plugin.Sample.Messaging.Policies
{
    using Sitecore.Commerce.Core;
    
    public class PluginPolicy : Policy
    {
        public PluginPolicy()
        {
            this.Icon = "data";
            this.IsDisabled = true;
        }
        
        public string Icon { get; set; }
        
        public bool IsDisabled { get; set; }
    }
}
