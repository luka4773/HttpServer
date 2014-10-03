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
           ///Methods for multithreading - command out if you don't want the multithread server to run(don't forget to reconfig bools in httpserver class).
           //http.StaticForMultiThread(http);
           //http.ListenForClients();
            http.RunServer();
            Console.WriteLine("Server started.");
           ///SUMMARY
           /// HTTP Server Assignment
           /// http://laerer.rhs.dk/andersb/sodpcs/httpserver/exerciseHttpServer.html
           /// Completed steps : 1,2,3,4,5,9,10
           /// Step 10 doesn't seem to work allthough im 100% sure the code is correct... please check it out and let me know what did i do wrong at lukas.cech12@gmail.com.
           /// 2/5 unit tests work
           /// I'm sorry I couldn't do anymore - time was running short and I didn't know how to implement it.
           /// SUMMARY        
        }
    }
}
