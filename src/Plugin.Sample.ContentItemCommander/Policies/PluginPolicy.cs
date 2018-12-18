
namespace Plugin.Sample.ContentItemCommander.Policies
{
    using Sitecore.Commerce.Core;
    
    public class PluginPolicy : Policy
    {
        public PluginPolicy()
        {
            this.Icon = "data";
        }
        
        public string Icon { get; set; }
    }
}
