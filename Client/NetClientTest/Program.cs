using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client;
using fastJSON;

namespace NetClientTest
{
    class Program
    {

        static void Main(string[] args)
        {
            var client = new NetClient("192.168.33.55");

            client.ResponseEvent += (o, e) => Console.WriteLine("server answer is " + e.Response);
            //client.NetErrorEvent += (o, e) => Console.WriteLine("server error is " + e.Error);

            client.Start();

            while (true)
            {
                var command = Console.ReadLine();

                if (command == "exit")
                    break;

                var operand = Console.ReadLine();
                var dict = new Dictionary<string, string>();

                dict[command] = operand;

                var json = JSON.Instance.ToJSON(dict);

                client.Send(json);
            }
            client.Stop();
        }
    }
}
