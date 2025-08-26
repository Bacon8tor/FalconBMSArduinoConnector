using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace FalconBMSArduinoConnector
{
    internal class DCSConnector
    {

        public void startDCSListen()
        {
            TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 31090);
            listener.Start();

            Console.WriteLine("Server started.");

            while (true)
            {
                Console.WriteLine("Waiting for DCS connection...");
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("DCS connected :-)");

                StreamReader reader = new StreamReader(client.GetStream());
                StreamWriter writer = new StreamWriter(client.GetStream());

                string s = string.Empty;

                while (true)
                {
                    s = reader.ReadLine();
                    Console.WriteLine(s);
                    if (s == "exit") break;
                }
                reader.Close();
                writer.Close();
                client.Close();
            }
        }
    }
}
