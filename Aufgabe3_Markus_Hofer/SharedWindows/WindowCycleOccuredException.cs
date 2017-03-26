//-----------------------------------------------------------------------
// <copyright file="WindowCycleOccuredException.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents an exception which will be thrown if there is a cycle in the network.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This class represents an exception which will be thrown if there is a cycle in the network.
    /// </summary>
    [Serializable]
    internal class WindowCycleOccuredException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowCycleOccuredException"/> class.
        /// </summary>
        public WindowCycleOccuredException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowCycleOccuredException"/> class.
        /// </summary>
        /// <param name="message">The message of the exception.</param>
        public WindowCycleOccuredException(string message) : base(message)
        {
        }
    }
}