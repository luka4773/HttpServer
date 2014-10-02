﻿using System;
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
            Socket sock = serverSocket.AcceptSocket();
            this.shutdownThread = new Thread(new ThreadStart(ListenForShutdown));
            this.shutdownThread.Start();
            

          

            while (Static)
            {
                
               ///Simple static server
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
               ///Static server with dynamic response, if the request line isn't empty simply split the string after the "/" which is present in every link and show the rest as what we requested. Lenght -5 cause it displays 5 characters that are not needed
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
                    ///a bit more complicated but managed to do something with it...always gives a blank page whenever there's an exception. No idea at all why.
                   TcpClient tcp = serverSocket.AcceptTcpClient();
                   Console.WriteLine("Dynamic client connected, lets go.");
                   Stream server = tcp.GetStream();
                   StreamReader sr = new StreamReader(server);
                   StreamWriter sw = new StreamWriter(server);
                   string s = sr.ReadLine();
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
                       
                       Console.WriteLine(fullfile);
                       sw.Write("HTTP/1.0 200 OK");                       
                       
                       stream.CopyTo(sw.BaseStream);
                       stream.Close();
                       

                   }
                   catch (FileNotFoundException)
                   {
                       Console.WriteLine("exception");
                       sw.Write("HTTP/1.0 404 Not Found");
                       sw.Write("\r\n");
                       
                       
                      
                       
                   }
                   catch (DirectoryNotFoundException)
                   {
                       Console.WriteLine("exception2");
                       sw.Write("HTTP/1.0 404 Directory Not Found");
                       sw.Write("\r\n");
                       
                   }
                   catch (Exception)
                   {
                       Console.WriteLine("exception3");
                       sw.Write("HTTP/1.0 400 Illegal request");
                       sw.Write("\r\n");
                       
                   }
                   finally
                   {
                       Console.WriteLine("finally");
                       sw.AutoFlush = true;
                       tcp.Close();
                       
                       
                   }
                   
               }
           
              
               
           
        
        }

       

        ///Method that runs a simple static server this one writes 100, 000 lines both to console and browser(desktop pc is too fast to have smaller number to test threads)
        public void StaticForMultiThread(object client)
        {
            TcpClient tcp = serverSocket.AcceptTcpClient();
            Console.WriteLine("Client connected, lets go.");
            Stream server = tcp.GetStream();
            StreamReader sr = new StreamReader(server);
            StreamWriter sw = new StreamWriter(server);
            /// 100, 000 for testing purposes, seems to be working ;)
            for (var i = 0; i < 100000; i++)
            {
                sw.Write("Hey, the time of your connection was: " + DateTime.Now + "\n");
                Console.WriteLine("Hey, they time of your connection was: " + DateTime.Now + "\n");
            }

            string request = sr.ReadLine();

            sw.AutoFlush = true;
            tcp.Close();
        }

        ///Method that accepts incoming clients and creates new threads for them as they come.
       public void ListenForClients()
       {
         

           while (true)
           {
               serverSocket.Start();
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
             
             Console.WriteLine("Shutting down server");
            Stop();
         }
    
    public void Stop() 
    {               
        System.Environment.Exit(1);
    }
    }
}
