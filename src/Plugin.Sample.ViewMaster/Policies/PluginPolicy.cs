
namespace Plugin.Sample.ViewMaster.Policies
{
    using Sitecore.Commerce.Core;
    
    public class PluginPolicy : Policy
    {
        public PluginPolicy()
        {
            this.Icon = "emoticon_cool";
            this.IsDisabled = true;
        }
        
        public string Icon { get; set; }
        
        public bool IsDisabled { get; set; }
    }
}
