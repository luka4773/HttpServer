using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpServer http = new HttpServer();
            ///Methods for multithreading - command out if you don't want the multithread server to run.
           // http.StaticForMultiThread(http);
           // http.ListenForClients();
            Console.WriteLine("Server started.");
            
        }
    }
}
