//-----------------------------------------------------------------------
// <copyright file="IWindow.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>Classes, which implement this interface, can be used as shared virtual window.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Classes, which implement this interface, can be used as shared virtual window.
    /// </summary>
    public interface IWindow
    {
        /// <summary>
        /// Gets called when the window received a message.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> OnMessageReceived;

        /// <summary>
        /// Gets called when the target of a message is this window.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> OnMessageDelivered;

        /// <summary>
        /// Gets called when the window forwards a message.
        /// </summary>
        event EventHandler<MessageReceivedEventArgs> OnMessageForwarded;

        /// <summary>
        /// Gets the ID of the window.
        /// </summary>
        /// <returns>The ID of the window.</returns>
        int GetID();

        /// <summary>
        /// Gets the unique ID of the window.
        /// </summary>
        /// <returns>The unique ID of the window.</returns>
        Guid GetUniqueID();

        /// <summary>
        /// Sets the cluster identifier of the window.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        void SetClusterIdentifier(object identifier);

        /// <summary>
        /// Gets the cluster identifier from the window.
        /// </summary>
        /// <returns>The cluster identifier.</returns>
        object GetClusterIdentifier();

        /// <summary>
        /// Gets the parent of the window.
        /// </summary>
        /// <returns>The parent of the window.</returns>
        IWindow GetParent();

        /// <summary>
        /// Gets the children of the window.
        /// </summary>
        /// <returns>The children of the window.</returns>
        List<IWindow> GetChildren();

        /// <summary>
        /// Sends the given message to this window.
        /// </summary>
        /// <param name="message">The given message.</param>
        void Send(WindowMessage message);

        /// <summary>
        /// Sets the given window as the parent of this window.
        /// </summary>
        /// <param name="parent">The window, which will be the new parent.</param>
        /// <param name="notifyChange">Indicates whether the other windows should be notified about the new node.</param>
        void SetParent(IWindow parent, bool notifyChange);

        /// <summary>
        /// Adds the given child to the window.
        /// </summary>
        /// <param name="child">The given child.</param>
        /// <param name="notifyChange">Indicates whether the other windows should be notified about the new node.</param>
        void AddChild(IWindow child, bool notifyChange);

        /// <summary>
        /// Removes the parent of this window.
        /// </summary>
        /// <param name="notifyChange">Indicates whether the other windows should be notified about the deleted parent.</param>
        void RemoveParent(bool notifyChange);

        /// <summary>
        /// Removes the given child from the window.
        /// </summary>
        /// <param name="child">The given child.</param>
        /// <param name="notifyChange">Indicates whether the other windows should be notified about the deleted node.</param>
        void RemoveChild(IWindow child, bool notifyChange);

        /// <summary>
        /// Calculates the total amount of connections for this window.
        /// </summary>
        /// <returns>The total amount of connections for this window.</returns>
        int GetTotalAmountOfConnections();

        /// <summary>
        /// Checks if any sub child or parent of this window contains the given window.
        /// </summary>
        /// <param name="window">The given window.</param>
        /// <param name="ignored">A list of windows, which will be ignored during scanning.</param>
        /// <returns>A boolean indicating whether the window exists or not.</returns>
        bool HasSubChildren(IWindow window, List<IWindow> ignored);
    }
}