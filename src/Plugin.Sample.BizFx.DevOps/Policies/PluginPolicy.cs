namespace Plugin.Sample.BizFx.DevOps.Policies
{
    using Sitecore.Commerce.Core;
    
    public class PluginPolicy : Policy
    {
        public PluginPolicy()
        {
            this.Icon = "plug_usb";
        }

        public string Icon { get; set; }
    }
}
