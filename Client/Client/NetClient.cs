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
        private readonly TcpClient tcpClient;
        private readonly NetworkStream stream;
        private readonly Thread thread;
        private bool threadWorking = true;

        /// <summary>
        /// This event is called when client receives server response
        /// </summary>
        public event ResponseEventHandler ResponseEvent;

        public NetClient(int port = 9505)
        {
            tcpClient = new TcpClient("localhost", port);
            stream = tcpClient.GetStream();

            thread = new Thread(Work) { IsBackground = false };
            thread.Start();
        }

        public void SendCmd(string command)
        {
            var output = new StreamWriter(stream);
            output.WriteLine(command);
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
            var input = new StreamReader(stream);

            while (threadWorking)
            {
                var response = input.ReadLine();

                if(ResponseEvent != null)
                    ResponseEvent(this, new ResponseEventArgs(response));
            }

            stream.Close();
            tcpClient.Close();
        }
    }
}
