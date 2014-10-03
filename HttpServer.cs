using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class HttpServer
    {
        private static readonly string RootCatalog = @"C:\Users\Lukas\Documents\";
        public  static int DefaultPort = 8080;
        private static readonly int ShutdownPort = 8081;
        private bool Static = false;
        private bool Dynamic = false;
        private bool openFile = true;
        private bool shutdown = true;
        static IPAddress localAddress = IPAddress.Parse("127.0.0.1");
        TcpListener serverSocket = new TcpListener(localAddress, DefaultPort);        
        private Thread listenThread;
        private Thread shutdownThread;
        
        
        

        /// Most of this project is within 1 constructor so far, switching between different types (dynamic, static, open file etc.) is done @ changing the respective booleans(static/dynamic/openfile etc.)

        public HttpServer(int port)
        {
           
            DefaultPort = port;

        }

        public void RunServer()
        {
            serverSocket.Start();
            Console.WriteLine("Server is up.");
            Program.Log.WriteEntry("Server started.");  
            this.shutdownThread = new Thread(new ThreadStart(ListenForShutdown));
            this.shutdownThread.Start();
            

          

            while (Static)
            {
                
               ///Simple static server
                    TcpClient tcp = serverSocket.AcceptTcpClient();
                    Console.WriteLine("Client connected, lets go.");
                    Program.Log.WriteEntry("Client request received.");
                    Stream server = tcp.GetStream();
                    Program.Log.WriteEntry("Static server started.");
                    StreamReader sr = new StreamReader(server);
                    StreamWriter sw = new StreamWriter(server);

                    sw.Write("Hey, the time of your connection was: " + DateTime.Now);                                   
                    string request = sr.ReadLine();
                    Program.Log.WriteEntry("Response sent to client.");
                    
                    sw.AutoFlush = true;
                    tcp.Close();
                

            }

           while (Dynamic)
           {
               ///Static server with dynamic response, if the request line isn't empty simply split the string after the "/" which is present in every link and show the rest as what we requested. Lenght -5 cause it displays 5 characters that are not needed
              TcpClient tcp = serverSocket.AcceptTcpClient();
               Console.WriteLine("Dynamic client connected, lets go.");
               Program.Log.WriteEntry("Dynamic server started.");
               Stream server = tcp.GetStream();
               StreamReader sr = new StreamReader(server);
               StreamWriter sw = new StreamWriter(server);
               string s = sr.ReadLine();
               Program.Log.WriteEntry("Client request received.");
             
               if (s != null)
               {
                   s = s.Split('/')[1];
                   s = s.Remove(s.Length - 5);
               }
               sw.Write("Hey, the file you have requested was: " + s + " at " + DateTime.Now);
               Program.Log.WriteEntry("Response sent to client.");
             
               sw.AutoFlush = true;
               tcp.Close();
           }


                while (openFile)
               {
                    ///a bit more complicated but managed to do something with it...
                   TcpClient tcp = serverSocket.AcceptTcpClient();
                   Console.WriteLine("Dynamic client connected, lets go.");
                   Stream server = tcp.GetStream();
                   Program.Log.WriteEntry("OpenFile server started.");
                   StreamReader sr = new StreamReader(server);
                   StreamWriter sw = new StreamWriter(server);
                   sw.AutoFlush = true;
                   string s = sr.ReadLine();
                   Program.Log.WriteEntry("Client request received.");
                   Console.WriteLine("asdad: " + s);
                   if (s == null)
                   {
                       Console.WriteLine("NULL");
                       tcp.Close();
                       continue;
                   }
                   string[] file = s.Split('/');
                   string filename = file[1];
                   string fullfile = RootCatalog + filename;
                   fullfile = fullfile.Remove(fullfile.Length - 5);              
                   Console.WriteLine(fullfile);
                   FileStream stream = null;
                   try
                   {
                      
                       stream = new FileStream(fullfile, FileMode.Open, FileAccess.Read);
                       Program.Log.WriteEntry("Response sent to client.");
                       Console.WriteLine(fullfile);
                    
                       sw.Write("HTTP/1.0 200 OK\r\n");
                    
                                             
                       
                       stream.CopyTo(sw.BaseStream);
                       stream.Close();
                       

                   }
                   catch (FileNotFoundException)
                   {
                       Console.WriteLine("exception");
                       sw.Write("HTTP/1.0 404 Not Found");
                       sw.Write("\r\n");
                       sw.Write("HTTP/1.0 404 Not found");
                       
                       
                      
                       
                   }
                   catch (DirectoryNotFoundException)
                   {
                       Console.WriteLine("exception2");
                       sw.Write("HTTP/1.0 404 Directory Not Found");
                       sw.Write("\r\n");
                       sw.Write("Directory not found.");
                       
                   }
                   catch (Exception)
                   {
                       Console.WriteLine("exception3");
                       sw.Write("HTTP/1.0 400 Illegal request");
                       sw.Write("\r\n");
                       sw.Write("Illegal request.");
                       
                   }
                   finally
                   {
                       Console.WriteLine("finally");
                       
                       tcp.Close();
                       
                       
                   }
                   
               }
           
              
               
           
        
        }

       

        ///Method that runs a simple static server this one writes 100, 000 lines both to console and browser(desktop pc is too fast to have smaller number to test threads)
        public void StaticForMultiThread(object client)
        {
            TcpClient tcp = serverSocket.AcceptTcpClient();
            Console.WriteLine("Client connected, lets go.");
            Program.Log.WriteEntry("Static server to test multithreading started.");
            Stream server = tcp.GetStream();
            StreamReader sr = new StreamReader(server);
            StreamWriter sw = new StreamWriter(server);
            Program.Log.WriteEntry("Client request received.");
            /// 100, 000 for testing purposes, seems to be working ;)
            for (var i = 0; i < 100000; i++)
            {
                sw.Write("Hey, the time of your connection was: " + DateTime.Now + "\n");
                Console.WriteLine("Hey, they time of your connection was: " + DateTime.Now + "\n");
            }
            Program.Log.WriteEntry("Response sent to client.");
            string request = sr.ReadLine();
            

            sw.AutoFlush = true;
            tcp.Close();
        }

        ///Method that accepts incoming clients and creates new threads for them as they come.
       public void ListenForClients()
       {

           serverSocket.Start();
           while (shutdown)
           {
               
               Program.Log.WriteEntry("Litenforclients for multithreaded server started.");
               //blocks until a client has connected to the server
               TcpClient tcp = this.serverSocket.AcceptTcpClient();
               this.listenThread = new Thread(new ThreadStart(ListenForClients));
               this.listenThread.Start();
               //create a thread to handle communication 
               //with connected client
               Thread clientThread = new Thread(new ParameterizedThreadStart(StaticForMultiThread));
               clientThread.Start(tcp);
           }
       }

       
         TcpListener shutdownSocket = new TcpListener(localAddress, ShutdownPort); 
        private void ListenForShutdown()
        {
             shutdownSocket.Start();
             TcpClient shutdownClient = this.shutdownSocket.AcceptTcpClient();
             Program.Log.WriteEntry("ListenForShutdown method started.");
             Console.WriteLine("Shutting down server");
            Stop();
         }
    
    public void Stop() 
    {

        Static = false;
        Dynamic = false;
        openFile = false;
        shutdown = false;
        IPAddress ip = IPAddress.Parse("127.0.0.1");
        TcpClient clientSocket = new TcpClient();
        clientSocket.Connect(ip, DefaultPort);
        Program.Log.WriteEntry("Server stopped.");
        serverSocket.Stop();
    }
    }
}
