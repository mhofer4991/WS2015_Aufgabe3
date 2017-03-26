//-----------------------------------------------------------------------
// <copyright file="InternalNetworkWindow.cs" company="Markus Hofer">
//     Copyright (c) Markus Hofer
// </copyright>
// <summary>This class represents an internal, local window, which accepts and communicates with external windows over the network.</summary>
//-----------------------------------------------------------------------
namespace Aufgabe3_Markus_Hofer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This class represents an internal, local window, which accepts and communicates with external windows over the network.
    /// </summary>
    [Serializable]
    public class InternalNetworkWindow : BasicWindow
    {
        /// <summary> The listener of the window. </summary>
        [NonSerialized]
        private TcpListener listener;

        /// <summary>
        /// Initializes a new instance of the <see cref="InternalNetworkWindow"/> class.
        /// </summary>
        /// <param name="id">The ID of the window.</param>
        public InternalNetworkWindow(int id) : base(id)
        {
        }

        /// <summary>
        /// Delegate for event OnStartSucceed.
        /// </summary>
        public delegate void StartSucceed();

        /// <summary>
        /// Delegate for event OnStartFailedDueToAlreadyUsedPort.
        /// </summary>
        public delegate void StartFailedDueToAlreadyUsedPort();

        /// <summary>
        /// Delegate for event OnConnectionSucceed.
        /// </summary>
        public delegate void ConnectionSucceed();

        /// <summary>
        /// Delegate for event OnConnectionFailedDueToCycle.
        /// </summary>
        public delegate void ConnectionFailedDueToCycle();

        /// <summary>
        /// Delegate for event OnConnectionFailedDueToExceed.
        /// </summary>
        public delegate void ConnectionFailedDueToExceed();

        /// <summary>
        /// Delegate for event OnConnectionFailedDueToUnknown.
        /// </summary>
        public delegate void ConnectionFailedDueToUnknown();

        /// <summary>
        /// Gets called when the window has been started and listens to connecting windows.
        /// </summary>
        [field: NonSerialized]
        public event StartSucceed OnStartSucceed;

        /// <summary>
        /// Gets called when the window could not be started because the given port is already used.
        /// </summary>
        [field: NonSerialized]
        public event StartFailedDueToAlreadyUsedPort OnStartFailedDueToAlreadyUsedPort;

        /// <summary>
        /// Gets called when the window connected successfully to another window.
        /// </summary>
        [field: NonSerialized]
        public event ConnectionSucceed OnConnectionSucceed;

        /// <summary>
        /// Gets called when the window could not connect because it would create a cycle.
        /// </summary>
        [field: NonSerialized]
        public event ConnectionFailedDueToCycle OnConnectionFailedDueToCycle;

        /// <summary>
        /// Gets called when the window could not connect because it would exceed the maximum amount of windows.
        /// </summary>
        [field: NonSerialized]
        public event ConnectionFailedDueToExceed OnConnectionFailedDueToExceed;

        /// <summary>
        /// Gets called when the window could not connect because the other window could not be found.
        /// </summary>
        [field: NonSerialized]
        public event ConnectionFailedDueToUnknown OnConnectionFailedDueToUnknown;

        /// <summary> Gets a value indicating whether the window is listening or not. </summary>
        /// <value> A boolean indicating whether the window is listening or not. </value>
        public bool IsListening { get; private set; }

        /// <summary>
        /// Connects to window with the given IP address and port.
        /// </summary>
        /// <param name="address">The given IP address.</param>
        /// <param name="port">The given port.</param>
        public void ConnectToExternalWindow(IPAddress address, int port)
        {
            TcpClient client = new TcpClient();
            IPEndPoint windowEndPoint = new IPEndPoint(address, port);

            client.BeginConnect(address, port, new AsyncCallback(this.Connected), client);
        }

        /// <summary>
        /// Starts the listener of the window with the given port.
        /// </summary>
        /// <param name="port">The given port of the listener.</param>
        public void Start(int port)
        {
            this.listener = new TcpListener(IPAddress.Any, port);

            try
            {
                this.listener.Start();

                this.IsListening = true;
                this.listener.BeginAcceptTcpClient(new AsyncCallback(this.ClientAccepted), this.listener);

                if (this.OnStartSucceed != null)
                {
                    this.OnStartSucceed();
                }
            }
            catch (SocketException)
            {
                if (this.OnStartFailedDueToAlreadyUsedPort != null)
                {
                    this.OnStartFailedDueToAlreadyUsedPort();
                }
            }
        }

        /// <summary>
        /// Stops the listener of the window.
        /// </summary>
        public void Stop()
        {
            this.listener.Stop();
            this.IsListening = false;
        }

        /// <summary>
        /// Gets called when the window connected to an external window.
        /// </summary>
        /// <param name="result">The result of the TCP client.</param>
        private void Connected(IAsyncResult result)
        {
            TcpClient client = (TcpClient)result.AsyncState;

            try
            {
                client.EndConnect(result);

                Thread thread = new Thread(new ParameterizedThreadStart(this.HandleExternalWindow));
                thread.IsBackground = true;
                thread.Start(client);
            }
            catch (SocketException)
            {
                if (this.OnConnectionFailedDueToUnknown != null)
                {
                    this.OnConnectionFailedDueToUnknown();
                }
            }
        }

        /// <summary>
        /// Handles the communication of an external window, which accepted our connection.
        /// </summary>
        /// <param name="args">The TCP client of the external window.</param>
        private void HandleExternalWindow(object args)
        {
            TcpClient client = (TcpClient)args;

            NetworkStream stream = client.GetStream();

            // Send connect request and it's own instance.
            byte[] request = new byte[1] { (byte)NetworkMessageCode.Connect_request };

            stream.Write(request, 0, request.Length);
            stream.Flush();

            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, this);

            stream.Flush();

            byte[] response = new byte[1];

            if (stream.Read(response, 0, response.Length) == response.Length)
            {
                if (response[0] == (byte)NetworkMessageCode.Connect_accepted)
                {
                    formatter = new BinaryFormatter();

                    IWindow window = (IWindow)formatter.Deserialize(stream);

                    ExternalNetworkWindow wi = new ExternalNetworkWindow(window, client);
                    wi.SetOpposite(this);

                    this.SetParent(wi, true);

                    wi.CheckAliveStatus();

                    if (this.OnConnectionSucceed != null)
                    {
                        this.OnConnectionSucceed();
                    }
                }
                else if (response[0] == (byte)NetworkMessageCode.Connect_denied_due_to_cycle)
                {
                    if (this.OnConnectionFailedDueToCycle != null)
                    {
                        this.OnConnectionFailedDueToCycle();
                    }
                }
                else if (response[0] == (byte)NetworkMessageCode.Connect_denied_due_to_exceed)
                {
                    if (this.OnConnectionFailedDueToExceed != null)
                    {
                        this.OnConnectionFailedDueToExceed();
                    }
                }
            }
        }

        /// <summary>
        /// Gets called after an external window connected to this window.
        /// </summary>
        /// <param name="result">The results of the TCP listener.</param>
        private void ClientAccepted(IAsyncResult result)
        {
            TcpListener li = (TcpListener)result.AsyncState;
            
            try
            {
                if (li.Server.IsBound)
                {
                    TcpClient client = li.EndAcceptTcpClient(result);

                    Thread thread = new Thread(new ParameterizedThreadStart(this.HandleClient));
                    thread.IsBackground = true;
                    thread.Start(client);
                }
            }
            catch (NullReferenceException)
            {
            }
        }

        /// <summary>
        /// Handles the communication of an external window, which connected to this window.
        /// </summary>
        /// <param name="args">The TCP client of the external window.</param>
        private void HandleClient(object args)
        {
            TcpClient client = (TcpClient)args;
            NetworkStream stream = client.GetStream();

            byte[] request = new byte[1];

            if (stream.Read(request, 0, request.Length) == request.Length)
            {
                if (request[0] == (byte)NetworkMessageCode.Connect_request)
                {
                    IFormatter formatter = new BinaryFormatter();

                    IWindow window = (IWindow)formatter.Deserialize(stream);

                    byte[] response = new byte[0];

                    try
                    {
                        // Set the parent of the external window, which can lead to two exceptions.
                        ExternalNetworkWindow external = new ExternalNetworkWindow(window, client);
                        external.SetOpposite(this);
                        external.SetParent(this, false);

                        this.AddChild(external, false);

                        // Send an acknowledge back.
                        response = new byte[1] { (byte)NetworkMessageCode.Connect_accepted };

                        stream.Write(response, 0, response.Length);
                        stream.Flush();

                        formatter = new BinaryFormatter();

                        formatter.Serialize(stream, this);
                        stream.Flush();

                        external.CheckAliveStatus();                        
                    }
                    catch (WindowCycleOccuredException)
                    {
                        response = new byte[] { (byte)NetworkMessageCode.Connect_denied_due_to_cycle };

                        stream.Write(response, 0, response.Length);
                        stream.Flush();
                    }
                    catch (TooManyWindowsException)
                    {
                        response = new byte[] { (byte)NetworkMessageCode.Connect_denied_due_to_exceed };

                        stream.Write(response, 0, response.Length);
                        stream.Flush();
                    }
                }
            }

            this.listener.BeginAcceptTcpClient(new AsyncCallback(this.ClientAccepted), this.listener);
        }
    }
}