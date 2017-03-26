//-----------------------------------------------------------------------
// <copyright file="WindowMessage.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents a message, which can be exchanged between windows.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents a message, which can be exchanged between windows.
    /// </summary>
    [Serializable]
    public class WindowMessage
    {
        /// <summary>The content of the message. </summary>
        private object content;

        /// <summary> The sender of the message. </summary>
        private IWindow sender;

        /// <summary> The transfer type of the message. </summary>
        private WindowMessageTransferType transferType;

        /// <summary> Tells if the message expects a response. </summary>
        private bool expectsResponse;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowMessage"/> class.
        /// </summary>
        /// <param name="id">The ID of the message.</param>
        /// <param name="sourceWindow">The source window.</param>
        /// <param name="targetWindow">The target window.</param>
        /// <param name="code">The code of the message.</param>
        /// <param name="status">The status of the message.</param>
        public WindowMessage(Guid id, IWindow sourceWindow, IWindow targetWindow, WindowMessageCode code, WindowMessageStatus status)
        {
            this.SourceWindow = sourceWindow;
            this.TargetWindow = targetWindow;
            this.Status = status;
            this.Code = code;
            this.ID = id;

            this.transferType = WindowMessageTransferType.Broadcast;
            this.Route = new Stack<Guid>();
            this.expectsResponse = true;
        }

        /// <summary> Gets the ID of the window. </summary>
        /// <value> The ID of the window. </value>
        public Guid ID { get; private set; }

        /// <summary> Gets the source window. </summary>
        /// <value> The source window. </value>
        public IWindow SourceWindow { get; private set; }

        /// <summary> Gets the target window. </summary>
        /// <value> The target window. </value>
        public IWindow TargetWindow { get; private set; }

        /// <summary> Gets the type of the message. </summary>
        /// <value> The type of the message. </value>
        public WindowMessageStatus Status { get; private set; }

        /// <summary> Gets the code of the message. </summary>
        /// <value> The code of the message. </value>
        public WindowMessageCode Code { get; private set; }

        /// <summary> Gets the route of the message. </summary>
        /// <value> The route of the message. </value>
        public Stack<Guid> Route { get; private set; }

        /// <summary> Gets or sets the content of the message. </summary>
        /// <value> The content of the message. </value>
        public object Content
        {
            get
            {
                return this.content;
            }

            set
            {
                this.content = value;
            }
        }

        /// <summary> Gets or sets the sender of the message. </summary>
        /// <value> The sender of the message. </value>
        public IWindow Sender
        {
            get
            {
                return this.sender;
            }

            set
            {
                this.sender = value;
            }
        }

        /// <summary> Gets or sets the transfer type of the message. </summary>
        /// <value> The transfer type of the message. </value>
        public WindowMessageTransferType TransferType
        {
            get
            {
                return this.transferType;
            }

            set
            {
                this.transferType = value;
            }
        }

        /// <summary> Gets or sets a value indicating whether the message expects a response or not. </summary>
        /// <value> A value indicating whether the message expects a response or not. </value>
        public bool ExpectsResponse
        {
            get
            {
                return this.expectsResponse;
            }

            set
            {
                this.expectsResponse = value;
            }
        }

        /// <summary>
        /// Deserializes the given byte array and returns a window message.
        /// </summary>
        /// <param name="content">The content represented as an array of bytes.</param>
        /// <returns>A window message.</returns>
        public static WindowMessage DeserializeByteArray(byte[] content)
        {
            WindowMessage msg;
            BinaryFormatter bf = new BinaryFormatter();

            using (var memStream = new MemoryStream())
            {
                memStream.Write(content, 0, content.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                msg = (WindowMessage)bf.Deserialize(memStream);
            }

            return msg;
        }

        /// <summary>
        /// Sets the route of the message.
        /// </summary>
        /// <param name="route">The route.</param>
        public void SetRoute(Stack<Guid> route)
        {
            this.Route = new Stack<Guid>(route.Reverse());
        }

        /// <summary>
        /// Creates and returns a copy of the message.
        /// </summary>
        /// <returns>A copy of the message.</returns>
        public WindowMessage GetCopy()
        {
            WindowMessage message = new WindowMessage(this.ID, this.SourceWindow, this.TargetWindow, this.Code, this.Status);
            message.TransferType = this.transferType;
            message.SetRoute(this.Route);
            message.Sender = this.sender;
            message.Content = this.content;
            message.ExpectsResponse = this.expectsResponse;

            return message;
        }

        /// <summary>
        /// Serializes the message to a byte array and returns it.
        /// </summary>
        /// <returns>An array of bytes representing the message.</returns>
        public byte[] GetSerializedByteArray()
        {
            BinaryFormatter bf = new BinaryFormatter();
            byte[] content;

            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, this);
                content = ms.ToArray();
            }

            return content;
        }
    }
}
