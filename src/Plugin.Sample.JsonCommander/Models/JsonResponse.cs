
namespace Plugin.Sample.JsonCommander.Models
{
    using Newtonsoft.Json;

    using Sitecore.Commerce.Core;
    
    public class JsonResponse : Model
    {
        public JsonResponse()
        {
            this.Json = string.Empty;
        }

        public string Json { get; set; }
        
        public string Uri { get; set; }
        
        public JsonTextReader Reader { get; set; }
    }
}
