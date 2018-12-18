namespace Plugin.Sample.ListMaster.EntityViews
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    using global::Plugin.Sample.ListMaster.Entities;
    using global::Plugin.Sample.Messaging.Components;
    using global::Plugin.Sample.Messaging.Models;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Framework.Pipelines;
    
    [PipelineDisplayName("DoActionImportList")]
    public class DoActionImportList : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionImportList(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

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

                var directorySelected = new DirectoryInfo(path);

                foreach (var fileToProcess in directorySelected.GetFiles("*.json"))
                {
                    var fileName = fileToProcess.Name;
                    var fullFilePath = path + @"\" + fileName;

                    var fileJson = File.ReadAllText(path + @"\" + fileName);

                    var serializerSettings = new JsonSerializerSettings
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
                        var historyComponent = managedList.GetComponent<HistoryComponent>();
                        historyComponent.History.Add(new HistoryEntryModel { Name = "StartImport",
                            EventMessage = $"Start Import of Pack File:{incomingManagedList.Name} ({fullFilePath})" });
                        await this._commerceCommander
                            .PersistEntity(context.CommerceContext, managedList);
                    }

                    var updatedEntities = 0;
                    var skippedEntities = 0;
                    var newEntities = 0;
                    var versionMismatchEntities = 0;

                    var tasks = new List<Task>();

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

                            using (var md5Hash = MD5.Create())
                            {
                                var persistedEntityHash = GetMd5Hash(md5Hash, persistedEntityJson);

                                if (VerifyMd5Hash(md5Hash, incomingEntityJson, persistedEntityHash))
                                {
                                    skippedEntities++;
                                }
                                else
                                {
                                    context.Logger.LogInformation($"The hashes are not same.  Updating Entity:{incomingEntity.Id}");
                                    await this._commerceCommander
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
                        var historyComponent = managedList.GetComponent<HistoryComponent>();
                        historyComponent.History.Add(new HistoryEntryModel
                        {
                            Name = "EndImport",
                            EventMessage = $"End Import of Pack File:{incomingManagedList.Name} ({fullFilePath}) New: {newEntities} Updated: {updatedEntities} Mismatch Version: {versionMismatchEntities} Skipped: {skippedEntities}"
                        });
                        await this._commerceCommander
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
        
        static string GetMd5Hash(HashAlgorithm md5Hash, string input)
        {
            var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            
            var sBuilder = new StringBuilder();
            
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(i.ToString("x2"));
            }
            
            return sBuilder.ToString();
        }
        
        static bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            var hashOfInput = GetMd5Hash(md5Hash, input);
            
            var comparer = StringComparer.OrdinalIgnoreCase;

            return 0 == comparer.Compare(hashOfInput, hash);
        }
    }
}
