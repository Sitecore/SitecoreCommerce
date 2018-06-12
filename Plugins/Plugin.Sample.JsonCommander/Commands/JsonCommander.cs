
namespace Plugin.Sample.JsonCommander
{
    using System;
    using System.Threading.Tasks;
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using System.IO;
    using System.Net;
    using System.Net.Http;

    /// <summary>
    /// Defines the JsonCommander command.
    /// </summary>
    public class JsonCommander : CommerceCommand
    {

        private static readonly JsonSerializer Serializer = new JsonSerializer
        {
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore
        };

        private static JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore,
            MaxDepth = 100,
            Formatting = Formatting.Indented

        };



        /// <summary>
        /// Initializes a new instance of the <see cref="JsonCommander"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider</param>
        public JsonCommander(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        /// <summary>
        /// Gets stanrd Serializer Settings
        /// </summary>
        /// <param name="commerceContext"></param>
        /// <returns></returns>
        public JsonSerializerSettings GetSerializerSettings(CommerceContext commerceContext)
        {
            return serializerSettings;
        }

        /// <summary>
        /// The process of the command
        /// </summary>
        /// <param name="commerceContext">
        /// The commerce context
        /// </param>
        /// <param name="uri">
        /// The uri for the command
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<JsonResponse> Process(CommerceContext commerceContext, string uri)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                JsonResponse response = new JsonResponse {Uri = uri };

                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        response.Json = await httpClient.GetStringAsync(response.Uri);
                    }

                    var reader = new StringReader(response.Json);

                    response.Reader = new JsonTextReader(reader);
                }
                catch (Exception ex)
                {
                    await commerceContext.AddMessage(
                            commerceContext.GetPolicy<KnownResultCodes>().Error,
                            $"JsonCommand.Get.Exception: Message={ex.Message}",
                            new object[] { ex },
                            $"JsonCommand.Get.Exception: Message={ex.Message}|Stack={ex.StackTrace}");

                    response = null;
                }
                return response;
            }
        }

        /// <summary>
        /// The process of the command
        /// </summary>
        /// <param name="commerceContext">
        /// The commerce context
        /// </param>
        /// <param name="uri">
        /// The uri for the command
        /// </param>
        /// <param name="body">
        /// The body for the command
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<JsonResponse> Post(CommerceContext commerceContext, string uri, string body)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                JsonResponse response = new JsonResponse { Uri = uri };

                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        var postResponse = await httpClient.PostAsJsonAsync(response.Uri, body);
                        response.Json = await postResponse.Content.ReadAsStringAsync();
                    }

                    var reader = new StringReader(response.Json);

                    response.Reader = new JsonTextReader(reader);
                }
                catch (Exception ex)
                {
                    await commerceContext.AddMessage(
                            commerceContext.GetPolicy<KnownResultCodes>().Error,
                            "JsonCommand",
                            new object[] { ex },
                            $"JsonCommand.Post.Exception: Message={ex.Message}|Stack={ex.StackTrace}");

                    response = null;
                }
                return response;
            }
        }

        /// <summary>
        /// The process of the command
        /// </summary>
        /// <param name="commerceContext">
        /// The commerce context
        /// </param>
        /// <param name="uri">
        /// The uri for the command
        /// </param>
        /// <param name="body">
        /// The body for the command
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<JsonResponse> Put(CommerceContext commerceContext, string uri, JsonAction body)
        {
            using (var activity = CommandActivity.Start(commerceContext, this))
            {
                JsonResponse response = new JsonResponse { Uri = uri };

                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        //var jsonAction = new JsonAction { environment = "HabitatShops" };

                        var postResponse = await httpClient.PutAsJsonAsync(response.Uri, body);
                        response.Json = await postResponse.Content.ReadAsStringAsync();
                    }

                    var reader = new StringReader(response.Json);

                    response.Reader = new JsonTextReader(reader);
                }
                catch (Exception ex)
                {
                    await commerceContext.AddMessage(
                            commerceContext.GetPolicy<KnownResultCodes>().Error,
                            "FailedToDeserialize",
                            new object[] { ex },
                            $"GetProdPadCommand.Exception: Message={ex.Message}|Stack={ex.StackTrace}");
                    commerceContext.Logger.LogWarning($"Exception: {ex.Message}");

                    response = null;
                }
                return response;
            }
        }

    }
}