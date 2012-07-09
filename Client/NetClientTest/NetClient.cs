using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client
{
    public delegate void ResponseEventHandler(object sender, ResponseEventArgs e);

    public class ResponseEventArgs : EventArgs
    {
        public ResponseEventArgs(string response)
        {
            Response = response;
        }

        public string Response { get; protected set; }
    }

    /// <summary>
    /// This class is used to work with server over network. Required server work, in other case will throw exception
    /// </summary>
    public class NetClient
    {
        public string Host { get; private set; }

        public int Port { get; private set; }

        /// <summary>
        /// This event is called when client receives server response
        /// </summary>
        public event ResponseEventHandler ResponseEvent;

        private readonly TcpClient tcpClient;
        private readonly NetworkStream stream;
        private readonly Thread thread;
        private bool threadWorking = true;
        private readonly int sleepTimeout;
        private const int BufferSize = 4096;

        public NetClient(string host = "localhost", int port = 9505, int sleepTimeout = 50)
        {
            Host = host;
            Port = port;
            this.sleepTimeout = sleepTimeout;

            tcpClient = new TcpClient(host, port);
            stream = tcpClient.GetStream();

            thread = new Thread(Work) { IsBackground = false };
            thread.Start();
        }

        public void Send(string command)
        {
            command = command.Trim(new[] {' ', '\n', '\t'});

            var output = new StreamWriter(stream);
            output.Write(command);
            output.Flush();
        }

        /// <summary>
        /// Is used to close connection to server
        /// </summary>
        public void Close()
        {
            // Writing bool variable is a atomic operation
            threadWorking = false;
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
            Send("");
            stream.Close();
            tcpClient.Close();
        }
    }
}
