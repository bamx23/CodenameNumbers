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
            var gClient = new GameClient("192.168.33.55");

            gClient.NetworkClient.ResponseEvent += (o, e) => Console.WriteLine("server answer is " + e.Message());
            gClient.NetworkClient.ResponseErrorEvent += (o, e) => Console.WriteLine("Response error occuried: " + e.Error);
            gClient.NetErrorEvent += (o, e) => Console.WriteLine("Net error occuried: " + e.Error);
            gClient.LoginEvent += (o, e) => { if (e.Ok) Console.WriteLine("Login ok"); else Console.WriteLine("Login not ok: " + e.Error); };

            gClient.Start();

            gClient.Login("semen", "megapassword");
            Thread.Sleep(1000);
            gClient.GameSessionList();
            Thread.Sleep(1000);
            gClient.GameSessionJoin(1);
            Thread.Sleep(1000);
            gClient.GameHit(27);
            Thread.Sleep(1000);
            gClient.GameLeave();

            Console.ReadLine();
            gClient.Stop();

        }
    }
}
