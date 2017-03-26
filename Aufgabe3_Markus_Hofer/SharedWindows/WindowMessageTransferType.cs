//-----------------------------------------------------------------------
// <copyright file="WindowMessageTransferType.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This enumeration contains different transfer types of a message.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This enumeration contains different transfer types of a message.
    /// </summary>
    [Serializable]
    public enum WindowMessageTransferType
    {
        /// <summary>
        /// Indicates that the route of the message should be used.
        /// </summary>
        UseRoute = 1,

        /// <summary>
        /// Indicates that all members of a network should get the message.
        /// </summary>
        Broadcast = 2,

        /// <summary>
        /// Indicates that only those childs get a message who actually contain the target node.
        /// </summary>
        LookForChilds = 3
    }
}
