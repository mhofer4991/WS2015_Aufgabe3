//-----------------------------------------------------------------------
// <copyright file="TooManyWindowsException.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents an exception which will be thrown if there are too many windows in a network.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This class represents an exception which will be thrown if there are too many windows in a network.
    /// </summary>
    [Serializable]
    internal class TooManyWindowsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TooManyWindowsException"/> class.
        /// </summary>
        public TooManyWindowsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TooManyWindowsException"/> class.
        /// </summary>
        /// <param name="message">The message of the exception.</param>
        public TooManyWindowsException(string message) : base(message)
        {
        }
    }
}