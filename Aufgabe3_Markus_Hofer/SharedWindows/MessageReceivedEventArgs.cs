//-----------------------------------------------------------------------
// <copyright file="MessageReceivedEventArgs.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>Event args, which will be passed when a window received a message.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Event args, which will be passed when a window received a message.
    /// </summary>
    [Serializable]
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message which has been received.</param>
        public MessageReceivedEventArgs(WindowMessage message)
        {
            this.Message = message;
        }

        /// <summary> Gets the received message. </summary>
        /// <value> The received message. </value>
        public WindowMessage Message { get; private set; }
    }
}
