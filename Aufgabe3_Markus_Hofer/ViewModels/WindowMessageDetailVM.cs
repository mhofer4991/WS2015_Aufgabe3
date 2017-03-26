//-----------------------------------------------------------------------
// <copyright file="WindowMessageDetailVM.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents a view model, which will be used to display information about a message.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents a view model, which will be used to display information about a message.
    /// </summary>
    public class WindowMessageDetailVM : INotifyPropertyChanged
    {
        /// <summary> The ID of the message. </summary>
        private string id;

        /// <summary> The sender ID of the message. </summary>
        private string senderID;

        /// <summary> The source ID of the message. </summary>
        private string sourceID;

        /// <summary> The target ID of the message. </summary>
        private string targetID;

        /// <summary> The content of the message represented as a string. </summary>
        private string content;

        /// <summary> The message which will be represented by this view model. </summary>
        private WindowMessage message;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowMessageDetailVM"/> class.
        /// </summary>
        public WindowMessageDetailVM()
        {
        }

        /// <summary>
        /// Gets called when a property has been changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary> Gets or sets the ID of the message. </summary>
        /// <value>The ID of the message.</value>
        public string ID
        {
            get
            {
                return this.id;
            }

            set
            {
                this.id = value;
                this.Notify();
            }
        }

        /// <summary> Gets or sets the sender ID of the message. </summary>
        /// <value>The sender ID of the message.</value>
        public string SenderID
        {
            get
            {
                return this.senderID;
            }

            set
            {
                this.senderID = value;
                this.Notify();
            }
        }

        /// <summary> Gets or sets the source ID of the message. </summary>
        /// <value>The source ID of the message.</value>
        public string SourceID
        {
            get
            {
                return this.sourceID;
            }

            set
            {
                this.sourceID = value;
                this.Notify();
            }
        }

        /// <summary> Gets or sets the target ID of the message. </summary>
        /// <value>The target ID of the message.</value>
        public string TargetID
        {
            get
            {
                return this.targetID;
            }

            set
            {
                this.targetID = value;
                this.Notify();
            }
        }

        /// <summary> Gets or sets the content of the message. </summary>
        /// <value>The content of the message.</value>
        public string Content
        {
            get
            {
                return this.content;
            }

            set
            {
                this.content = value;
                this.Notify();
            }
        }

        /// <summary>
        /// Sets the message which will be represented by this view model.
        /// </summary>
        /// <param name="message">The message.</param>
        public void SetMessage(WindowMessage message)
        {
            this.message = message;

            this.ID = message.ID.ToString();
            this.SenderID = message.Sender.GetID().ToString();
            this.SourceID = message.SourceWindow.GetID().ToString();

            if (this.message.TargetWindow != null)
            {
                this.TargetID = message.TargetWindow.GetID().ToString();
            }
            else
            {
                this.TargetID = "All";
            }

            if (this.message.Code == WindowMessageCode.Text_message)
            {
                this.Content = (string)this.message.Content;
            }
            else if (this.message.Code == WindowMessageCode.Nodes_update)
            {
                this.Content = "Node updated: " + ((IWindow)this.message.Content).GetID();
            }
            else
            {
                this.Content = string.Empty;
            }
        }

        /// <summary>
        /// Notifies about changed properties.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        private void Notify([CallerMemberName]string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
