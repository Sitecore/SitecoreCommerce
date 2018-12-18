namespace Plugin.Sample.ListMaster.EntityViews
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using global::Plugin.Sample.ListMaster.Commands;
    using global::Plugin.Sample.ListMaster.Components;
    using global::Plugin.Sample.ListMaster.Entities;
    using global::Plugin.Sample.Messaging.Components;
    using global::Plugin.Sample.Messaging.Models;

    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.EntityViews;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Commerce.Plugin.SQL;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName("DoActionExportList")]
    public class DoActionPublishList : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;
        
        public DoActionPublishList(CommerceCommander commerceCommander)
        {
            this._commerceCommander = commerceCommander;
        }

        public override async Task<EntityView> Run(EntityView entityView, CommercePipelineExecutionContext context)
        {
            if (entityView == null
                || !entityView.Action.Equals("ListMaster-ExportList", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var listName = entityView.ItemId;
                var directoryPath = @"C:\Users\kha\Documents\ExportedEntities\";
                
                var asPack = entityView.Properties.First(p => p.Name == "AsPack").Value ?? "";
                var incremental = entityView.Properties.First(p => p.Name == "Incremental").Value ?? "";

                var listFriendlyName = entityView.ItemId.Replace("Entity-ManagedList-", "");
                
                var asPackBool = System.Convert.ToBoolean(asPack);
                var incrementalBool = System.Convert.ToBoolean(incremental);

                var listCount = await this._commerceCommander.Command<GetListCountCommand>().Process(context.CommerceContext, listFriendlyName);
                
                var listMembers = new StringBuilder();
                var listMembersSerialized = new StringBuilder();

                var packContainer = new PackContainer();

                var managedList = await this._commerceCommander.Command<GetManagedListCommand>().Process(context.CommerceContext, listFriendlyName);
                if (managedList != null)
                {
                    var publishedListEntities = managedList.GetComponent<ListEntitiesInPublish>();
                    publishedListEntities.LastPublishStart = DateTimeOffset.UtcNow;
                    publishedListEntities.LastPublishCount = 0;
                    if (!incrementalBool)
                    {
                        publishedListEntities.LastRow = 0;
                        publishedListEntities.PublishCycle++;
                        publishedListEntities.PublishIteration = 0;
                    }
                    else
                    {
                        publishedListEntities.PublishIteration++;
                    }
                    var historyComponent = managedList.GetComponent<HistoryComponent>();
                    historyComponent.History.Add(new HistoryEntryModel { Name = "StartPublish", EventMessage = $"Start Publish of List:{listName} ({listCount})" });
                    var managedListPersistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, managedList);

                    JsonSerializerSettings serializerSettings = new JsonSerializerSettings
                    {
                        TypeNameHandling = TypeNameHandling.All,
                        NullValueHandling = NullValueHandling.Ignore,
                        MaxDepth = 100,
                        Formatting = Formatting.Indented
                    };

                    while (publishedListEntities.LastRow < listCount)
                    {
                        var arg = new FindEntitiesInListArgument(typeof(CommerceEntity), listFriendlyName, publishedListEntities.LastRow, 100);
                        var result = await this._commerceCommander.Pipeline<FindEntitiesInListPipeline>().Run(arg, context.CommerceContext.GetPipelineContextOptions());

                        foreach (var listEntity in result.List.Items)
                        {
                            var serializedEntity = JsonConvert.SerializeObject(listEntity, serializerSettings);
                            
                            listMembersSerialized.Append(serializedEntity + ",");
                            managedList.TotalItemCount++;

                            packContainer.Entities.Add(listEntity);

                            listMembers.AppendLine();

                            publishedListEntities.LastPublishCount++;
                        }

                        publishedListEntities.LastRow = publishedListEntities.LastRow + 100;
                    }

                    if (asPackBool)
                    {
                        packContainer.ManagedLists.Add(managedList);

                        var serializedPackContainer = JsonConvert.SerializeObject(packContainer, serializerSettings);
                        var writePath = directoryPath + $"{context.CommerceContext.Environment.Name}-Pack-" + managedList.Id + $"_{DateTime.UtcNow.ToString("yyyy-MM-dd-hh-mm")}.json";
                        if (incrementalBool)
                        {
                            writePath = directoryPath + $"{context.CommerceContext.Environment.Name}.{managedList.Name}-Pack-{publishedListEntities.PublishCycle.ToString("00#")}-{publishedListEntities.PublishIteration.ToString("00#")}-{DateTime.UtcNow.ToString("yyyy-MM-dd-hh-mm")}.json";
                        }
                        else
                        {
                            writePath = directoryPath + $"{context.CommerceContext.Environment.Name}.{managedList.Name}-Pack-{publishedListEntities.PublishCycle.ToString("00#")}-000-{DateTime.UtcNow.ToString("yyyy-MM-dd-hh-mm")}.json";
                        }
                        File.WriteAllText(writePath, serializedPackContainer);
                        
                        this._commerceCommander.Command<GzipCommand>().Compress(context.CommerceContext, new DirectoryInfo(directoryPath));
                    }

                    historyComponent.History.Add(new HistoryEntryModel { Name = "ListPublish", EventMessage = $"Published List:{listName} ({listCount})" });

                    publishedListEntities.LastPublishEnd = DateTimeOffset.UtcNow;

                    await this._commerceCommander.PersistEntity(context.CommerceContext, managedList);
                }
            }
            catch (Exception ex)
            {
                context.Logger.LogError($"Catalog.DoActionAddList.Exception: Message={ex.Message}");
            }

            return entityView;
        }
    }
}
