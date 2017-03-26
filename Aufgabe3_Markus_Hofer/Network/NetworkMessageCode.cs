//-----------------------------------------------------------------------
// <copyright file="NetworkMessageCode.cs" company="Markus Hofer">
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
    public enum NetworkMessageCode
    {
        /// <summary>
        /// Indicates that the sender wants to connect.
        /// </summary>
        Connect_request = 1,

        /// <summary>
        /// Indicates that the sender accepted the request.
        /// </summary>
        Connect_accepted = 2,

        /// <summary>
        /// Indicates that the sender denied the request due to a cycle.
        /// </summary>
        Connect_denied_due_to_cycle = 3,

        /// <summary>
        /// Indicates that the sender denied the request because the amount of windows exceeded the maximum.
        /// </summary>
        Connect_denied_due_to_exceed = 4,

        /// <summary>
        /// Indicates that the sender wants to tell that he is still alive.
        /// </summary>
        Keep_alive = 5,

        /// <summary>
        /// Indicates that the sender sends a window message.
        /// </summary>
        Window_message_transfer = 6
    }
}
