using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    public delegate void MessageEventHandler(object sender, MessageEventArgs e);

    public delegate void NetErrorEventHandler(object sender, NetErrorEventArgs e);

    public class NetErrorEventArgs : EventArgs
    {
        public NetErrorEventArgs(string error)
        {
            Error = error;
        }

        public string Error { get; protected set; }
    }

    public class MessageEventArgs : EventArgs
    {
        public byte[] ByteMessage { get; protected set; }

        public Encoding DefaultEncoding { get; protected set; }

        public MessageEventArgs(byte[] message, Encoding defaultEncoding)
        {
            ByteMessage = message;
            DefaultEncoding = defaultEncoding;
        }

        /// <summary>
        /// Using Default encoding(ASCII)
        /// </summary>
        /// <returns></returns>
        public string Message()
        {
            return Message(Encoding.ASCII);
        }

        public string Message(Encoding encoding)
        {
            return encoding.GetString(ByteMessage);
        }
    }

    public enum NetClientStatus { Working, Stopped };

    /// <summary>
    /// This class is used to work with server over network. Required server work, in other case will throw exception
    /// </summary>
    public class NetClient
    {
        /// <summary>
        /// Current client status(Working or Stopped)
        /// </summary>
        public NetClientStatus Status { get; protected set; }

        public string Host { get; protected set; }

        public int Port { get; protected set; }

        public int SleepTimeout { get; protected set; }

        public Encoding DefaultEncoding { get; protected set; }

        public NetProtocol Protocol { get; protected set; }

        public bool FireResponseEventAsync { get; protected set; }

        /// <summary>
        /// This event is called when client receives server response
        /// </summary>
        public event MessageEventHandler ResponseEvent;

        /// <summary>
        /// This event is called when Send() Method invokes before sending operation. Data contains sending message
        /// </summary>
        public event MessageEventHandler SendEvent;

        /// <summary>
        /// This event is called immediately after connectetion creation
        /// </summary>
        public event EventHandler StartEvent;

        /// <summary>
        /// This event is called before connection close
        /// </summary>
        public event EventHandler StopEvent;

        public event NetErrorEventHandler ResponseErrorEvent;

        /*/// <summary>
        /// This event is called if some net-connected error occured 
        /// </summary>
        public event NetErrorEventHandler NetErrorEvent;*/

        protected TcpClient tcpClient;
        protected NetworkStream stream;
        protected Thread workThread;     

        protected bool threadWorking = true;

        //----------------------------------------------------
        // This variables are used for async Response event
        protected bool ResponseProcessedCompleted
        {
            get { return responseProcessedResult == null || responseProcessedResult.IsCompleted; }
        }
        protected IAsyncResult responseProcessedResult = null;
        protected Queue<MessageEventArgs> responseQueue;
        //----------------------------------------------------

        /// <summary>
        /// Creates new NetClient. Client is created stopped. To start it call Start() method
        /// </summary>
        /// <param name="host">Server's host</param>
        /// <param name="port">Port used to connect server</param>
        /// <param name="defaultEncoding">Encoding that convert string to Byte array Encoding. (ASCII, Unicode)...</param>
        /// <param name="sleepTimeout">Timeout of trying to read server's response</param>
        /// <param name="protocol">Client is using NetProtocol by default(if protocol = null)</param>
        /// <param name="fireResponseEventAsync">If it is true ResponseEvent will be called in additional thread </param>
        public NetClient(string host = "localhost", int port = 9505, NetProtocol protocol = null, 
            Encoding defaultEncoding = null, int sleepTimeout = 50, bool fireResponseEventAsync = false)
        {
            Host = host;
            Port = port;
            SleepTimeout = sleepTimeout;
            Protocol = protocol ?? new NetProtocol();
            Status = NetClientStatus.Stopped;
            DefaultEncoding = defaultEncoding ?? Encoding.ASCII;
            FireResponseEventAsync = fireResponseEventAsync;
        }

        /// <summary>
        /// Creates connection with server
        /// </summary>
        public void Start()
        {
            if (Status == NetClientStatus.Working)
                throw new Exception("Cannot start server because it is already working");

            tcpClient = new TcpClient(Host, Port);
            stream = tcpClient.GetStream();
            Status = NetClientStatus.Working;

            if (StartEvent != null)
                StartEvent(this, EventArgs.Empty);

            workThread = new Thread(Work) {IsBackground = false};
            workThread.Start();

        }

        /// <summary>
        /// Stops the client work and closes connection to server
        /// </summary>
        public void Stop()
        {
            if (StopEvent != null)
                StopEvent(this, EventArgs.Empty);

            // empty string is a sign for server to close the connection
            Send("");

            // Writing bool variable is a atomic operation
            threadWorking = false;

            Status = NetClientStatus.Stopped;
            stream.Close();
            tcpClient.Close();
        }

        public void Send(string message)
        {
            Send(message, DefaultEncoding);
        }

        public void Send(string message, Encoding encoding)
        {
            Send(encoding.GetBytes(message));
        }

        public virtual void Send(byte[] message)
        {
            if (Status == NetClientStatus.Stopped)
                throw new Exception("Cannot send a message, while server is stopped");

            if (SendEvent != null)
                SendEvent(this, new MessageEventArgs(message, DefaultEncoding));

            var output = new BinaryWriter(stream);

            Protocol.Send(message, output);
            output.Flush();
        }

        public void Send(dynamic message)
        {
            Send(BitConverter.GetBytes(message));
        }

        /// <summary>
        /// Method is called in new thread
        /// </summary>
        protected virtual void Work()
        {
            if (FireResponseEventAsync)
                responseQueue = new Queue<MessageEventArgs>();

            while (threadWorking)
            {
                if (stream.DataAvailable)
                    ProcessResponse();
                else if (responseQueue.Count != 0 && ResponseProcessedCompleted)
                    SendResponseEvent(responseQueue.Dequeue());
                else
                    Thread.Sleep(SleepTimeout);
            }
        }

        protected void ProcessResponse()
        {
            try
            {
                var buffer = Protocol.Receive(stream, tcpClient);
                if (ResponseEvent != null)
                {
                    var messageArgs = new MessageEventArgs(buffer, DefaultEncoding);
                    SendResponseEvent(messageArgs);
                }
            }
            catch (Exception e)
            {
                if (ResponseErrorEvent != null)
                    ResponseErrorEvent(this, new NetErrorEventArgs(e.Message));
            }
        }

        protected void SendResponseEvent(MessageEventArgs messageArgs)
        {
            if (FireResponseEventAsync)
            {
                if (ResponseProcessedCompleted)
                    responseProcessedResult = ResponseEvent.BeginInvoke(this, messageArgs, null, null);
                else
                    responseQueue.Enqueue(messageArgs);
            }
            else
                ResponseEvent(this, messageArgs);
        }
    }

    /// <summary>
    /// NetProtocol class is implements Send and Receive according to application net features
    /// </summary>
    public class NetProtocol
    {
        public virtual void Send(byte[] message, BinaryWriter output)
        {
            output.Write(message);
        }

        public virtual byte[] Receive(NetworkStream stream, TcpClient tcpClient)
        {
            var buffer = new byte[tcpClient.Available];
            stream.Read(buffer, 0, tcpClient.Available);

            return buffer;
        }

        public override string ToString()
        {
            return "NetProtocol";
        }
    }
}
