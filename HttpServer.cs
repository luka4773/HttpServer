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
        private static readonly string RootCatalog = @"C:\Users\Lukas\Documents";
        public static readonly int DefaultPort = 8080;
        private bool Static = false;
        private bool Dynamic = false;
        private bool openFile = true;
        
      
       public  HttpServer()
        {
            IPAddress localAddress = IPAddress.Parse("127.0.0.1");
            TcpListener serverSocket = new TcpListener(localAddress, DefaultPort);
            serverSocket.Start();
            Console.WriteLine("Server is up.");
            Socket sock = serverSocket.AcceptSocket();

            while (Static)
            {
                
               
                    TcpClient tcp = serverSocket.AcceptTcpClient();
                    Console.WriteLine("Client connected, lets go.");
                    Stream server = tcp.GetStream();
                    StreamReader sr = new StreamReader(server);
                    StreamWriter sw = new StreamWriter(server);
                    sw.Write("Hey, the time of your connection was: " + DateTime.Now);
                    string request = sr.ReadLine();
                    
                    sw.AutoFlush = true;
                    tcp.Close();
                

            }

           while (Dynamic)
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


                while (openFile)
               {
                   TcpClient tcp = serverSocket.AcceptTcpClient();
                   Console.WriteLine("Dynamic client connected, lets go.");
                   Stream server = tcp.GetStream();
                   StreamReader sr = new StreamReader(server);
                   StreamWriter sw = new StreamWriter(server);
                   string s = sr.ReadLine();
                   string[] file = s.Split(' ');
                   string filename = file[1];
                   string fullfile = RootCatalog + filename;

                   try
                   {
                       FileStream stream = new FileStream(fullfile, FileMode.Open, FileAccess.Read);
                       stream.CopyTo(sw.BaseStream);
                       stream.Close();
                   }
                   catch (FileNotFoundException)
                   {
                       sw.Write("HTTP/1.0 404 Not Found\n\r");
                       sw.Write("\r\n");
                       sw.Flush();
                   }
                   catch (DirectoryNotFoundException)
                   {
                       sw.Write("HTTP/1.0 404 Not Found\n\r");
                       sw.Write("\r\n");
                       sw.Flush();
                   }
                   sw.AutoFlush = true;
                   tcp.Close();
               }
           
              
               
           
        }
     
    }
}
