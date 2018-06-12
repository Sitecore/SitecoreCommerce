
namespace Plugin.Sample.JsonCommander
{
    using Sitecore.Commerce.Core;
    using System;

    /// <summary>
    /// ActionHistoryModel
    /// </summary>
    public class ActionHistoryModel : Model
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionHistoryModel"/> class.
        /// </summary>
        public ActionHistoryModel()
        {
            Id = "";
            Completed = DateTime.UtcNow;
            Response = "";
            JSON = "";
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; private set; }

        /// <summary>
        /// Response
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// Completed
        /// </summary>
        public DateTime Completed { get; set; }

        /// <summary>
        /// Json
        /// </summary>
        public string JSON { get; set; }

        /// <summary>
        /// EntityId
        /// </summary>
        public string EntityId { get; set; }

        /// <summary>
        /// ItemId
        /// </summary>
        public string ItemId { get; set; }
        
    }
}
