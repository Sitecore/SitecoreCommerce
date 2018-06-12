
namespace Plugin.Sample.JsonCommander
{
    using Newtonsoft.Json;
    using Sitecore.Commerce.Core;
    using System.Net.Http;

    /// <summary>
    /// JsonResponse
    /// </summary>
    public class JsonResponse : Model
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonResponse"/> class.
        /// </summary>
        public JsonResponse()
        {
            Json = "";
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Json { get; set; }

        /// <summary>
        /// uri
        /// </summary>
        public string Uri { get; set; }

        /// <summary>
        /// reader
        /// </summary>
        public JsonTextReader Reader { get; set; }

        /// <summary>
        /// Response
        /// </summary>
        public HttpResponseMessage Response { get; set; }

    }
}
