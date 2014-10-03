using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    class Program
    {
        public static EventLog Log;
        static void Main(string[] args)
        {
            if (!EventLog.SourceExists("HttpServer")) EventLog.CreateEventSource("HTTPServer", "Application");
            Log = new EventLog { Source = "HTTPServer" };
            HttpServer http = new HttpServer(8080);
            ///Methods for multithreading - command out if you don't want the multithread server to run.
           // http.StaticForMultiThread(http);
           // http.ListenForClients();
            http.RunServer();
            Console.WriteLine("Server started.");
            
        }
    }
}
