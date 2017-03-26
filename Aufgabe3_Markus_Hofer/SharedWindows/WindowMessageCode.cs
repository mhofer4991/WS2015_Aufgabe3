//-----------------------------------------------------------------------
// <copyright file="WindowMessageCode.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This enumeration contains different message codes.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This enumeration contains different message codes.
    /// </summary>
    [Serializable]
    public enum WindowMessageCode
    {
        /// <summary>
        /// Indicates that the content of the message is a text.
        /// </summary>
        Text_message = 1,

        /// <summary>
        /// Indicates that the content of the message is a node which has been updated.
        /// </summary>
        Nodes_update = 2,

        /// <summary>
        /// Indicates that the sender wants the list of messages of the receiver.
        /// </summary>
        List_of_messages = 3
    }
}
