//-----------------------------------------------------------------------
// <copyright file="WindowMessageStatus.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This enumeration contains different states of a message.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This enumeration contains different types of a message.
    /// </summary>
    [Serializable]
    public enum WindowMessageStatus
    {
        /// <summary>
        /// Indicates that the window message is a transfer with a content.
        /// </summary>
        Transfer,

        /// <summary>
        /// Indicates that the window message is an acknowledge to a previous message.
        /// </summary>
        Transfer_received,

        /// <summary>
        /// Indicates that a previous message could not reach his target.
        /// </summary>
        Transfer_failed
    }
}
