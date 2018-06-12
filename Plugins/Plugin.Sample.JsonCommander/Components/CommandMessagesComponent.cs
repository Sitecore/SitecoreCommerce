
namespace Plugin.Sample.JsonCommander
{
    using Sitecore.Commerce.Core;
    using System.Collections.Generic;

    /// <summary>
    /// The SampleComponent.
    /// </summary>
    public class CommandMessagesComponent : Component
    {
    
        /// <summary>
        /// ActionHistoryComponent
        /// </summary>
        public CommandMessagesComponent()
        {
            Messages = new List<CommandMessage>();
        }

        /// <summary>
        /// History
        /// </summary>
        public List<CommandMessage> Messages { get; set; }

        /// <summary>
        /// AddHistory
        /// </summary>
        /// <param name="message"></param>
        public void AddMessage(CommandMessage message)
        {
            Messages.Add(message);
        }
    }
}