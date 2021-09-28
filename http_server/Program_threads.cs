
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace http_server
{
    class Program
    {
        private Socket httpSocket;
        private int port = 80;
        private Thread thread1;
        private Thread thread2;
        private int requests_nr = 0;
        private int working_nr = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Program p = new Program();
            p.startServer();
            Console.ReadLine();
        }

        private void startServer()
        {
            httpSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);

            try
            {
                if (port > 65535 || port < 0) throw new Exception("Wrong port given");

                Console.WriteLine("Server Started on port:" + port);
                IPEndPoint endpoint = new IPEndPoint(IPAddress.Any, 80);
                httpSocket.Bind(endpoint);
                httpSocket.Listen(10);
                httpSocket.BeginAccept(new AsyncCallback(ThreadMethod), null);

                requests_nr++;




                Console.WriteLine("doing other thing");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void ThreadMethod(IAsyncResult AR)
        {
            try
            {
                httpSocket.EndAccept(AR);

                Socket client = httpSocket.Accept();

                while (true)
                {




                    working_nr++;
                    String data = "";
                    byte[] bytes = new byte[2048];


                    while (true)
                    {
                        int numBytes = client.Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, numBytes);
                        if (data.IndexOf("\r\n") > -1) break;

                    }
                    Console.WriteLine("Thread data: ");
                    Console.WriteLine(data);
                    string return_header = "HTTP/1.1 200 Everything is Fine\n Server: C#_httpServer\nContent-Type: text/html; charset: UTF-8\n\n";
                    string return_body = "<!DOCTYPE html> <html><head><title> My Server </title>  </head><body> aaa</body></html> ";
                    string return_string = return_header + return_body;
                    byte[] return_Data = Encoding.ASCII.GetBytes(return_string);
                    client.SendTo(return_Data, client.RemoteEndPoint);



                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);

            }
        }

    }
}