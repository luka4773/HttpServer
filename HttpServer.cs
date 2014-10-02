using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServer
{
    internal class HttpServer
    {

        public static readonly int DefaultPort = 8080;

      
       public  HttpServer()
        {
            IPAddress localAddress = IPAddress.Parse("127.0.0.1");
            TcpListener serverSocket = new TcpListener(localAddress, DefaultPort);
            serverSocket.Start();
            Console.WriteLine("Server is up.");
            Socket sock = serverSocket.AcceptSocket();

            while (true)
            {
                TcpClient tcp = serverSocket.AcceptTcpClient();
                Console.WriteLine("Client connected, lets go.");
                Stream server = tcp.GetStream();
                StreamReader sr = new StreamReader(server);
                StreamWriter sw = new StreamWriter(server);
                string request = sr.ReadLine();
                sw.Write("Hey, the time of your request was: " +DateTime.Now);
              
                sw.AutoFlush = true;
                tcp.Close();

            }
        }
     
    }
}
