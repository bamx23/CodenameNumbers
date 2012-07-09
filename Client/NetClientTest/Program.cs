using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client;

namespace NetClientTest
{
    class Program
    {

        static void Main(string[] args)
        {
            var client = new NetClient("192.168.33.55");
            client.ResponseEvent += client_ResponseEvent;
            var command = "";
            while (command != "exit")
            {
                command = Console.ReadLine();
                client.Send(command);
            }
            client.Close();
        }

        private static void client_ResponseEvent(object sender, ResponseEventArgs e)
        {
            Console.WriteLine("server answer is " + e.Response);
        }
    }
}
