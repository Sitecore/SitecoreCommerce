
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

    /// <summary>
    /// Defines the do action edit line item block.
    /// </summary>
    /// <seealso>
    ///     <cref>
    ///         Sitecore.Framework.Pipelines.PipelineBlock{Sitecore.Commerce.EntityViews.EntityView,
    ///         Sitecore.Commerce.EntityViews.EntityView, Sitecore.Commerce.Core.CommercePipelineExecutionContext}
    ///     </cref>
    /// </seealso>
    [PipelineDisplayName("DoActionExportList")]
    public class DoActionPublishList : PipelineBlock<EntityView, EntityView, CommercePipelineExecutionContext>
    {
        private readonly CommerceCommander _commerceCommander;

        /// <summary>
        /// Initializes a new instance of the <see cref="DoActionPublishList"/> class.
        /// </summary>
        /// <param name="commerceCommander">The <see cref="CommerceCommander"/> is a gateway object to resolving and executing other Commerce Commands and other control points.</param>
        public DoActionPublishList(CommerceCommander commerceCommander)
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
                || !entityView.Action.Equals("ListMaster-ExportList", StringComparison.OrdinalIgnoreCase))
            {
                return entityView;
            }

            try
            {
                var listName = entityView.ItemId;
                var directoryPath = @"C:\Users\kha\Documents\ExportedEntities\";

                //var asFiles = entityView.Properties.First(p => p.Name == "AsFiles").Value ?? "";
                var asPack = entityView.Properties.First(p => p.Name == "AsPack").Value ?? "";
                var incremental = entityView.Properties.First(p => p.Name == "Incremental").Value ?? "";

                var listFriendlyName = entityView.ItemId.Replace("Entity-ManagedList-", "");

                //var asFilesBool = System.Convert.ToBoolean(asFiles);
                var asPackBool = System.Convert.ToBoolean(asPack);
                var incrementalBool = System.Convert.ToBoolean(incremental);

                var listCount = await this._commerceCommander.Command<GetListCountCommand>().Process(context.CommerceContext, listFriendlyName);

                //var currentSkip = 0;
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
                    var historyComponent = managedList.GetComponent<Messaging.HistoryComponent>();
                    historyComponent.History.Add(new Messaging.HistoryEntryModel { Name = "StartPublish", EventMessage = $"Start Publish of List:{listName} ({listCount})" });
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

                            //var json = listEntity.Deflate();
                            listMembersSerialized.Append(serializedEntity + ",");

                            //if (asFilesBool)
                            //{
                            //    File.WriteAllText(directoryPath + listEntity.Id + $"_V{listEntity.Version}_{listEntity.DateUpdated?.ToString("yyyy-MM-dd-hh-mm")}.json", serializedEntity);
                            //}

                            managedList.TotalItemCount++;
                            //publishedListEntities.AddHistory(listEntity.Id);

                            packContainer.Entities.Add(listEntity);

                            listMembers.AppendLine();

                            publishedListEntities.LastPublishCount++;
                        }

                        publishedListEntities.LastRow = publishedListEntities.LastRow + 100;

                    }

                    //if (asFilesBool)
                    //{
                    //    var managedListJson = JsonConvert.SerializeObject(managedList, serializerSettings);
                    //    File.WriteAllText(directoryPath + managedList.Id + ".json", managedListJson);
                    //}

                    if (asPackBool)
                    {
                        //var serializedEntities = $"{{ Entities :[ {listMembersSerialized.ToString()} ]}}";
                        //File.WriteAllText(@"C:\Users\kha\Documents\ExportedEntities\aaPack-" + managedList.Id + $"_{DateTime.UtcNow.ToString("yyyy-MM-dd-hh-mm")}.json", serializedEntities);
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

                    historyComponent.History.Add(new Messaging.HistoryEntryModel { Name = "ListPublish", EventMessage = $"Published List:{listName} ({listCount})" });

                    publishedListEntities.LastPublishEnd = DateTimeOffset.UtcNow;

                    managedListPersistResult = await this._commerceCommander.PersistEntity(context.CommerceContext, managedList);
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
