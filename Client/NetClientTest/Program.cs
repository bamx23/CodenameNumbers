using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Client;
using fastJSON;

namespace NetClientTest
{
    class Program
    {

        static void Main(string[] args)
        {
            var gClient = new GameClient("luckygeck.dyndns-home.com");

            gClient.Client.ResponseEvent += (o, e) => Console.WriteLine("server answer is (" + e.Message() + ")");
            gClient.NetErrorEvent += (o, e) => Console.WriteLine("Net error occuried: " + e.Error);

            //gClient.Start();

            gClient.Login("username", "megapassword");
            Console.ReadLine();
            gClient.Stop();

        }
    }
}
