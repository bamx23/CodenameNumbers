using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    public delegate void ResponseEventHandler(object sender, ResponseEventArgs e);

    public delegate void SendEventHandler(object sender, SendEventArgs e);

    public delegate void NetErrorEventHandler(object sender, NetErrorEventArgs e);

    public class ResponseEventArgs : EventArgs
    {
        public ResponseEventArgs(string response)
        {
            Response = response;
        }

        public string Response { get; protected set; }
    }

    public class SendEventArgs : EventArgs
    {
        public SendEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; protected set; }
    }

    public class NetErrorEventArgs : EventArgs
    {
        public NetErrorEventArgs(string error)
        {
            Error = error;
        }

        public string Error { get; protected set; }
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

        /// <summary>
        /// This event is called when client receives server response
        /// </summary>
        public event ResponseEventHandler ResponseEvent;

        /// <summary>
        /// This event is called when Send() Method invokes before sending operation. Data contains sending message
        /// </summary>
        public event SendEventHandler SendEvent;

        /// <summary>
        /// This event is called immediately after connectetion creation
        /// </summary>
        public event EventHandler StartEvent;

        /// <summary>
        /// This event is called before connection close
        /// </summary>
        public event EventHandler StopEvent;

        /*/// <summary>
        /// This event is called if some net-connected error occured 
        /// </summary>
        public event NetErrorEventHandler NetErrorEvent;*/

        private TcpClient tcpClient;
        private NetworkStream stream;
        private Thread thread;
        private bool threadWorking = true;
        private readonly int sleepTimeout;
        private const int BufferSize = 4096;

        /// <summary>
        /// Creates new NetClient. Client is created stopped. To start it call Start() method
        /// </summary>
        /// <param name="host">Server's host</param>
        /// <param name="port">Port used to connect server</param>
        /// <param name="sleepTimeout">Timeout of trying to read server's response</param>
        public NetClient(string host = "localhost", int port = 9505, int sleepTimeout = 50)
        {
            Host = host;
            Port = port;
            this.sleepTimeout = sleepTimeout;
            Status = NetClientStatus.Stopped;
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

            thread = new Thread(Work) { IsBackground = false };
            thread.Start();

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

        public void Send(string message, bool trimMessage = true)
        {
            if (Status == NetClientStatus.Stopped)
                throw new Exception("Cannot send a message, while server is stopped");

            if (trimMessage)
                message = message.Trim(new[] { ' ', '\n', '\t' });

            if (SendEvent != null)
                SendEvent(this, new SendEventArgs(message));

            var output = new StreamWriter(stream);

            output.Write(message);
            output.Flush();
        }

        /// <summary>
        /// Method is called in new thread
        /// </summary>
        private void Work()
        {
            var buffer = new byte[BufferSize];

            while (threadWorking)
            {
                if (stream.DataAvailable)
                {
                    int length = tcpClient.Available;

                    stream.Read(buffer, 0, length);
                    var response = Encoding.ASCII.GetString(buffer, 0, length);

                    if (ResponseEvent != null)
                        ResponseEvent(this, new ResponseEventArgs(response));
                }
                else
                    Thread.Sleep(sleepTimeout);
            }
        }
    }
}
