using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HttpServer
{
    internal class HttpServer
    {

        public static readonly int DefaultPort = 8080;
        private bool staticDynamic = false;
      
       public  HttpServer()
        {
            IPAddress localAddress = IPAddress.Parse("127.0.0.1");
            TcpListener serverSocket = new TcpListener(localAddress, DefaultPort);
            serverSocket.Start();
            Console.WriteLine("Server is up.");
            Socket sock = serverSocket.AcceptSocket();

            while (staticDynamic)
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

           while (true)
           {
               TcpClient tcp = serverSocket.AcceptTcpClient();
               Console.WriteLine("Dynamic client connected, lets go.");
               Stream server = tcp.GetStream();
               StreamReader sr = new StreamReader(server);
               StreamWriter sw = new StreamWriter(server);
               string s = sr.ReadLine();
               if (s != null)
               {
                   s = s.Split('/')[1];
                   s = s.Remove(s.Length - 5);
               }
               sw.Write("Hey, the file you have requested was: " + s + " at " + DateTime.Now);

               sw.AutoFlush = true;
               tcp.Close();
           }
        }
     
    }
}
