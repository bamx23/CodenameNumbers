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
            var client = new NetClient("192.168.33.55");

            //client.ResponseEvent += (o, e) => Console.WriteLine("server answer is " + e.Response);
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

                var lst = new List<Dictionary<string, string>>();
                lst.Add(dict);

                var json = JSON.Instance.ToJSON(lst);

                while (true)
                {
                    client.Send(json);
                    Thread.Sleep(1);
                }
                
            }
            client.Stop();
        }
    }
}
