// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrackedList.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2017
// </copyright>----------------------------------------------------------------------------------------------------------

namespace Plugin.Sample.Orders.Enhancements
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Core.Commands;
    using Sitecore.Framework.Conditions;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the RunningCommands Model 
    /// </summary>
    /// <seealso cref="Sitecore.Commerce.Core.Model" />
    public class RunningCommands : Model
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RunningCommands"/> class.
        /// </summary>
        public RunningCommands()
        {
            History = new List<string>();
            //RunningActions = new List<CommerceCommand>();
        }

        /// <summary>
        /// History
        /// </summary>
        public List<string> History { get; set; }

        /// <summary>
        /// AddHistory
        /// </summary>
        /// <param name="history"></param>
        public void AddHistory(string history)
        {
            History.Add(history);
        }

        //public List<CommerceCommand> RunningActions { get; set; }

    }
}
