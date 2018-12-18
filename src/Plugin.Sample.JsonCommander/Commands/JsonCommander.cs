
namespace Plugin.Sample.JsonCommander.Commands
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using Plugin.Sample.JsonCommander.Models;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;

    public class JsonCommander : CommerceCommand
    {
        private static JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            NullValueHandling = NullValueHandling.Ignore,
            MaxDepth = 100,
            Formatting = Formatting.Indented
        };

        public JsonCommander(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<JsonResponse> Process(CommerceContext commerceContext, string uri)
        {
            using (CommandActivity.Start(commerceContext, this))
            {
                var response = new JsonResponse {Uri = uri };

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

        public async Task<JsonResponse> Put(CommerceContext commerceContext, string uri, JsonAction body)
        {
            var response = new JsonResponse { Uri = uri };
            using (CommandActivity.Start(commerceContext, this))
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
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