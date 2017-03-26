//-----------------------------------------------------------------------
// <copyright file="BasicWindow.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents the basic window, which already is able to route window messages in the network.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents the basic window, which already is able to route window messages in the network.
    /// </summary>
    [Serializable]
    public class BasicWindow : IWindow
    {
        /// <summary>
        /// Gets the default maximum amount of connected windows.
        /// </summary>
        public const int DefaultMaxAmountOfWindows = 102;

        /// <summary>
        /// Gets the ID of the window.
        /// </summary>
        private int id;

        /// <summary>
        /// Gets the unique ID of the window.
        /// </summary>
        private Guid uID;

        /// <summary>
        /// The parent of the window.
        /// </summary>
        private IWindow parent;

        /// <summary>
        /// The children of the window.
        /// </summary>
        private List<IWindow> children;

        /// <summary>
        /// The cluster identifier of the window.
        /// </summary>
        private object clusterIdentifier;

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicWindow"/> class.
        /// </summary>
        /// <param name="id">The ID of the window.</param>
        public BasicWindow(int id) : this(id, Guid.NewGuid())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicWindow"/> class.
        /// </summary>
        /// <param name="id">The ID of the window.</param>
        /// <param name="uniqueID">The unique ID of the window.</param>
        public BasicWindow(int id, Guid uniqueID)
        {
            this.id = id;
            this.parent = null;
            this.children = new List<IWindow>();

            this.uID = uniqueID;
            this.clusterIdentifier = 0;
        }

        /// <summary>
        /// Gets called when the window received a message.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<MessageReceivedEventArgs> OnMessageReceived;

        /// <summary>
        /// Gets called when the target of a message is this window.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<MessageReceivedEventArgs> OnMessageDelivered;

        /// <summary>
        /// Gets called when the window forwards a message.
        /// </summary>
        [field: NonSerialized]
        public event EventHandler<MessageReceivedEventArgs> OnMessageForwarded;

        /// <summary>
        /// Gets the ID of the window.
        /// </summary>
        /// <returns>The ID of the window.</returns>
        public int GetID()
        {
            return this.id;
        }

        /// <summary>
        /// Gets the unique ID of the window.
        /// </summary>
        /// <returns>The unique ID of the window.</returns>
        public Guid GetUniqueID()
        {
            return this.uID;
        }

        /// <summary>
        /// Sets the cluster identifier of the window.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        public void SetClusterIdentifier(object identifier)
        {
            this.clusterIdentifier = identifier;
        }

        /// <summary>
        /// Gets the cluster identifier from the window.
        /// </summary>
        /// <returns>The cluster identifier.</returns>
        public object GetClusterIdentifier()
        {
            return this.clusterIdentifier;
        }

        /// <summary>
        /// Gets the parent of the window.
        /// </summary>
        /// <returns>The parent of the window.</returns>
        public IWindow GetParent()
        {
            return this.parent;
        }

        /// <summary>
        /// Gets the children of the window.
        /// </summary>
        /// <returns>The children of the window.</returns>
        public List<IWindow> GetChildren()
        {
            return this.children.ToList();
        }

        /// <summary>
        /// Sends the given message to this window.
        /// </summary>
        /// <param name="message">The given message.</param>
        public virtual void Send(WindowMessage message)
        {
            if (message.Sender == null)
            {
                message.Sender = this;
            }

            // this.OnMessageReceived(this, new MessageReceivedEventArgs(message));
            this.HandleReceivedMessage(message);
        }

        /// <summary>
        /// Sets the given window as the parent of this window.
        /// </summary>
        /// <param name="parent">The window, which will be the new parent.</param>
        /// <param name="notifyChange">Indicates whether the other windows should be notified about the new node.</param>
        public void SetParent(IWindow parent, bool notifyChange)
        {
            bool cycle = false;
            IWindow descendant = parent;

            while (descendant != null)
            {
                if (descendant.GetUniqueID().Equals(this.GetUniqueID()))
                {
                    cycle = true;
                }

                descendant = descendant.GetParent();
            }

            if (cycle)
            {
                throw new WindowCycleOccuredException();
            }
            else if (this.GetTotalAmountOfConnections() + 2 > BasicWindow.DefaultMaxAmountOfWindows)
            {
                throw new TooManyWindowsException();
            }
            else
            {
                this.parent = parent;

                if (notifyChange)
                {
                    this.NotifyAboutUpdatedNodes();
                }
            }
        }

        /// <summary>
        /// Adds the given child to the window.
        /// </summary>
        /// <param name="child">The given child.</param>
        /// <param name="notifyChange">Indicates whether the other windows should be notified about the new node.</param>
        public void AddChild(IWindow child, bool notifyChange)
        {
            bool cycle = false;
            IWindow descendant = this;

            while (descendant != null)
            {
                if (descendant.GetUniqueID().Equals(child.GetUniqueID()))
                {
                    cycle = true;
                }

                descendant = descendant.GetParent();
            }

            if (cycle)
            {
                throw new WindowCycleOccuredException();
            }
            else if (this.GetTotalAmountOfConnections() + 2 > BasicWindow.DefaultMaxAmountOfWindows)
            {
                throw new TooManyWindowsException();
            }
            else
            {
                this.children.Add(child);

                if (notifyChange)
                {
                    this.NotifyAboutUpdatedNodes();
                }
            }
        }

        /// <summary>
        /// Removes the given child from the window.
        /// </summary>
        /// <param name="child">The given child.</param>
        /// <param name="notifyChange">Indicates whether the other windows should be notified about the deleted node.</param>
        public void RemoveChild(IWindow child, bool notifyChange)
        {
            if (this.children.RemoveAll(x => x.GetUniqueID().Equals(child.GetUniqueID())) != 0)
            {
                if (notifyChange)
                {
                    this.NotifyAboutUpdatedNodes();
                }
            }
        }

        /// <summary>
        /// Removes the parent of this window.
        /// </summary>
        /// <param name="notifyChange">Indicates whether the other windows should be notified about the deleted parent.</param>
        public void RemoveParent(bool notifyChange)
        {
            if (this.parent != null)
            {
                this.parent = null;

                if (notifyChange)
                {
                    this.NotifyAboutUpdatedNodes();
                }
            }                
        }

        /// <summary>
        /// Calculates the total amount of connections for this window.
        /// </summary>
        /// <returns>The total amount of connections for this window.</returns>
        public int GetTotalAmountOfConnections()
        {
            int total = 0;
            IWindow temp = this.GetParent();

            while (temp != null)
            {
                total++;
                temp = temp.GetParent();
            }

            total += this.GetTotalAmountOfChildren(this);

            return total;
        }

        /// <summary>
        /// Checks if any sub child or parent of this window contains the given window.
        /// </summary>
        /// <param name="window">The given window.</param>
        /// <param name="ignored">A list of windows, which will be ignored during scanning.</param>
        /// <returns>A boolean indicating whether the window exists or not.</returns>
        public bool HasSubChildren(IWindow window, List<IWindow> ignored)
        {
            // Merge parent and children
            List<IWindow> merged = this.GetChildren().ToList();

            if (this.parent != null)
            {
                merged.Add(this.parent);
            }

            // Check...
            bool has = false;

            foreach (IWindow node in merged)
            {
                if (ignored.FindIndex(x => x.GetID().Equals(node.GetID())) < 0)
                {
                    if (node.GetID().Equals(window.GetID()))
                    {
                        has = true;
                    }
                    else
                    {
                        has = has || node.HasSubChildren(window, new List<IWindow>() { this });
                    }
                }
            }

            return has;
        }

        /// <summary>
        /// Transfers the message to other windows by sending it to the parent and children.
        /// </summary>
        /// <param name="message">The message which will be transferred.</param>
        /// <returns>The amount of directly connected nodes which got the message.</returns>
        public int TransferMessage(WindowMessage message)
        {
            int count = 0;

            // Merge parent and children
            List<IWindow> merged = this.GetChildren().ToList();

            if (this.parent != null)
            {
                merged.Add(this.parent);
            }

            // Transfer...
            if (message.TransferType == WindowMessageTransferType.UseRoute)
            {
                if (message.Route.Count > 0)
                {
                    Guid temp = message.Route.Pop();
                    List<IWindow> found = merged.Where<IWindow>(x => x.GetUniqueID().Equals(temp)).ToList();

                    if (found.Count > 0)
                    {
                        message.Sender = this; 
                        found[0].Send(message);
                        count++;
                    }
                }
            }
            else if (message.TransferType == WindowMessageTransferType.Broadcast)
            {
                foreach (IWindow node in merged)
                {
                    if (!message.Sender.GetUniqueID().Equals(node.GetUniqueID()))
                    {
                        WindowMessage msgCopy = message.GetCopy();
                        msgCopy.Route.Push(this.GetUniqueID());
                        msgCopy.Sender = this;

                        node.Send(msgCopy);
                        count++;
                    }
                }
            }
            else if (message.TransferType == WindowMessageTransferType.LookForChilds)
            {
                List<IWindow> found = new List<IWindow>();

                foreach (IWindow child in this.GetChildren())
                {
                    if (child.GetID().Equals(message.TargetWindow.GetID()))
                    {
                        found.Add(child);
                    }
                    else if (child.HasSubChildren(message.TargetWindow, new List<IWindow>() { this }))
                    {
                        found.Add(child);
                    }
                }

                if (this.GetParent() != null)
                {
                    if (this.GetParent().GetID().Equals(message.TargetWindow.GetID()))
                    {
                        found.Add(this.GetParent());
                    }
                    else if (this.GetParent().HasSubChildren(message.TargetWindow, new List<IWindow>() { this }))
                    {
                        found.Add(this.GetParent());
                    }
                }

                WindowMessage msgCopy = message.GetCopy();
                msgCopy.Route.Push(this.GetUniqueID());
                msgCopy.Sender = this;

                foreach (IWindow find in found)
                {
                    if (!find.GetUniqueID().Equals(message.Sender.GetUniqueID()))
                    {
                        find.Send(msgCopy);
                        count++;
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Sets the parent of the window.
        /// </summary>
        /// <param name="parent">The parent of the window.</param>
        protected void SetParent(IWindow parent)
        {
            this.parent = parent;
        }

        /// <summary>
        /// Sets the children of the window.
        /// </summary>
        /// <param name="children">A list of child windows.</param>
        protected void SetChildren(List<IWindow> children)
        {
            this.children = children;
        }

        /// <summary>
        /// Handles a message, which has been received by this window.
        /// </summary>
        /// <param name="message">The message, which has been received.</param>
        private void HandleReceivedMessage(WindowMessage message)
        {
            WindowMessage temp = message.GetCopy();
            bool delivered = false;
            bool failed = false;

            if ((message.TargetWindow != null && message.TargetWindow.GetID().Equals(this.GetID())) ||
                    (message.TargetWindow == null && message.TransferType == WindowMessageTransferType.Broadcast))
            {
                // It is possible that the message will be changed by the method.
                this.HandleDeliveredMessage(message);

                delivered = true;
            }
            else if (this.OnMessageReceived != null)
            {
                this.OnMessageReceived(this, new MessageReceivedEventArgs(temp));
            }

            if (this.TransferMessage(message.GetCopy()) == 0)
            {
                failed = !delivered;
            }
            else if (this.OnMessageForwarded != null)
            {
                this.OnMessageForwarded(this, new MessageReceivedEventArgs(temp));
            }

            // message.Sender = this;
            if (failed)
            {
                if (message.ExpectsResponse)
                {
                    WindowMessage response = new WindowMessage(message.ID, this, message.SourceWindow, message.Code, WindowMessageStatus.Transfer_failed);
                    response.SetRoute(message.Route);
                    response.ExpectsResponse = false;
                    response.TransferType = WindowMessageTransferType.UseRoute;

                    this.TransferMessage(response);
                }
            }
        }

        /// <summary>
        /// Handles a message, which has been exactly to this window.
        /// </summary>
        /// <param name="message">The message, which has been exactly sent to this window.</param>
        private void HandleDeliveredMessage(WindowMessage message)
        {
            if (message.ExpectsResponse)
            {
                WindowMessage response = new WindowMessage(message.ID, this, message.SourceWindow, message.Code, WindowMessageStatus.Transfer_received);
                response.SetRoute(message.Route);
                response.ExpectsResponse = false;
                response.TransferType = WindowMessageTransferType.UseRoute;

                // this.Send(response);
                this.TransferMessage(response);
            }

            if (message.Code == WindowMessageCode.Nodes_update)
            {
                if (this.UpdateNodes((IWindow)message.GetCopy().Content))
                {
                    message.Content = this;

                    // It is the root. So he will send it back to all children.
                    if (this.GetParent() == null)
                    {
                        message.Sender = this;
                    }
                }
            }
            else
            {
            }

            if (this.OnMessageDelivered != null)
            {
                this.OnMessageDelivered(this, new MessageReceivedEventArgs(message.GetCopy()));
            }
        }

        /// <summary>
        /// Updates the given window.
        /// </summary>
        /// <param name="updated">The connected node which will be updated.</param>
        /// <returns>A boolean indicating whether a connected node has been updated or not.</returns>
        private bool UpdateNodes(IWindow updated)
        {
            // Merge parent and children
            List<IWindow> merged = this.GetChildren().ToList();

            if (this.parent != null)
            {
                merged.Add(this.parent);
            }

            // Update...
            if (this.parent != null && this.parent.GetUniqueID().Equals(updated.GetUniqueID()))
            {
                ((BasicWindow)this.parent).SetParent(updated.GetParent());
                ((BasicWindow)this.parent).SetChildren(updated.GetChildren());

                return true;
            }
            else
            {
                List<IWindow> found = this.children.Where(x => x.GetUniqueID().Equals(updated.GetUniqueID())).ToList();

                if (found.Count == 1)
                {
                    ((BasicWindow)found[0]).SetParent(updated.GetParent());
                    ((BasicWindow)found[0]).SetChildren(updated.GetChildren());

                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Notifies all connected nodes about the update.
        /// </summary>
        private void NotifyAboutUpdatedNodes()
        {
            WindowMessage note = new WindowMessage(Guid.NewGuid(), this, null, WindowMessageCode.Nodes_update, WindowMessageStatus.Transfer);
            note.Sender = this;
            note.ExpectsResponse = false;
            note.Content = this;
            note.TransferType = WindowMessageTransferType.Broadcast;

            // this.TransferMessage(note);
            this.Send(note);
        }

        /// <summary>
        /// Gets the total amount of children of the given window, including it's sub children.
        /// </summary>
        /// <param name="window">The given window.</param>
        /// <returns>The total amount of children of the given window.</returns>
        private int GetTotalAmountOfChildren(IWindow window)
        {
            int total = window.GetChildren().Count;

            foreach (IWindow child in window.GetChildren())
            {
                total += this.GetTotalAmountOfChildren(child);
            }

            return total;
        }
    }
}
