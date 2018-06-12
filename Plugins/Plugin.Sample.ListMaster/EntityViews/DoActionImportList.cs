
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Plugin.Sample.ListMaster
{
    using Sitecore.Commerce.Core;
    using Sitecore.Framework.Pipelines;
    using Sitecore.Commerce.EntityViews;
    using Microsoft.Extensions.Logging;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using System.Text;
    using System.IO;
    using Newtonsoft.Json;
    using Sitecore.Commerce.Plugin.SQL;
    using System.Security.Cryptography;
    using System.Collections.Generic;


    /// <summary>
    /// Defines the do action edit line item block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("DoActionImportList")]
    public class DoActionImportList : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionImportList"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionImportList(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="entityView">
        /// The argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="EntityView"/>.
        /// </returns>
        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {

            if (entityView == null
                || !entityView.Action.Equals("ListMaster-ImportList", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var path = entityView.Properties.First(p => p.Name == "Path").Value ?? "";

                DirectoryInfo directorySelected = new DirectoryInfo(path);

                foreach (FileInfo fileToProcess in directorySelected.GetFiles("*.json"))
                {
                    var fileName = fileToProcess.Name;
                    var fullFilePath = path + @"\" + fileName;

                    var fileJson = File.ReadAllText(path + @"\" + fileName);

                    JsonSerializerSettings serializerSettings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        NullValueHandling = NullValueHandling.Ignore,
                        MaxDepth = 100,
                        Formatting = Formatting.Indented
                    };

                    var packContainer = JsonConvert.DeserializeObject<PackContainer>(fileJson, serializerSettings);

                    foreach(var incomingManagedList in packContainer.ManagedLists)
                    {
                        var managedList = await this._commerceCommander.Command<GetManagedListCommand>()
                            .Process(context.CommerceContext, incomingManagedList.Name);
                        var historyComponent = managedList.GetComponent<Messaging.HistoryComponent>();
                        historyComponent.History.Add(new Messaging.HistoryEntryModel { Name = "StartImport",
                            EventMessage = $"Start Import of Pack File:{incomingManagedList.Name} ({fullFilePath})" });
                        var managedListPersistResult = await this._commerceCommander
                            .PersistEntity(context.CommerceContext, managedList);
                    }

                    var updatedEntities = 0;
                    var skippedEntities = 0;
                    var newEntities = 0;
                    var versionMismatchEntities = 0;

                    List<Task> tasks = new List<Task>();

                    foreach (var incomingEntity in packContainer.Entities)
                    {
                        var persistedEntity = await this._commerceCommander
                            .GetEntity<CommerceEntity>(context.CommerceContext, incomingEntity.Id);

                        if (persistedEntity == null)
                        {
                            context.Logger.LogInformation($"No Matching Persisted Entity.  Persisting Entity:{incomingEntity.Id}");
                            incomingEntity.IsPersisted = false;

                            tasks.Add(Task.Run(() => this._commerceCommander
                                .PersistEntity(context.CommerceContext, incomingEntity)));

                            newEntities++;
                        }
                        else if (persistedEntity.Version != incomingEntity.Version)
                        {
                            versionMismatchEntities++;
                        }
                        else
                        {
                            var incomingEntityJson = incomingEntity.Deflate();
                            var persistedEntityJson = persistedEntity.Deflate();

                            using (MD5 md5Hash = MD5.Create())
                            {
                                string persistedEntityHash = GetMd5Hash(md5Hash, persistedEntityJson);

                                if (VerifyMd5Hash(md5Hash, incomingEntityJson, persistedEntityHash))
                                {
                                    skippedEntities++;
                                }
                                else
                                {
                                    context.Logger.LogInformation($"The hashes are not same.  Updating Entity:{incomingEntity.Id}");
                                    var persistResult = await this._commerceCommander
                                        .PersistEntity(context.CommerceContext, incomingEntity);
                                    updatedEntities++;
                                }
                            }
                        }

                        if (tasks.Count > 100)
                        {
                            await Task.WhenAll(tasks);
                            tasks = new List<Task>();
                        }
                    }

                    await Task.WhenAll(tasks);

                    foreach (var incomingManagedList in packContainer.ManagedLists)
                    {
                        var managedList = await this._commerceCommander.Command<GetManagedListCommand>()
                            .Process(context.CommerceContext, incomingManagedList.Name);
                        var historyComponent = managedList.GetComponent<Messaging.HistoryComponent>();
                        historyComponent.History.Add(new Messaging.HistoryEntryModel
                        {
                            Name = "EndImport",
                            EventMessage = $"End Import of Pack File:{incomingManagedList.Name} ({fullFilePath}) New: {newEntities} Updated: {updatedEntities} Mismatch Version: {versionMismatchEntities} Skipped: {skippedEntities}"
                        });
                        var managedListPersistResult = await this._commerceCommander
                            .PersistEntity(context.CommerceContext, managedList);
                    }

                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionAddList.Exception: Message={ex.Message}");
            }

            return entityView;
        }


        static string GetMd5Hash(MD5 md5Hash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }



}
