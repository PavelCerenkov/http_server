using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace http_server
{
    class Program
    {
        private Socket httpSocket;
        private int port = 80;
        private static  int thread_count=10;
        private Thread[] threads=new Thread[thread_count+1];


        static void Main(string[] args)
        {
            Console.WriteLine("Server Starting");
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
                for (int i = 0; i < thread_count; i++)
                {
                    threads[i] = new Thread(() => ThreadMethod(i));
                    threads[i].Start();
                }
              
                Console.WriteLine("Threads Created Succesfully Waiting for requests");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        private void ThreadMethod(int nr)
        {
            
            try
            {

                while (true)
                {
                    Socket client = httpSocket.Accept();
                    {
                        String data = "";
                        byte[] bytes = new byte[2048];


                        while (true)
                        {
                            int numBytes = client.Receive(bytes);
                            data += Encoding.ASCII.GetString(bytes, 0, numBytes);
                            if (data.IndexOf("\r\n") > -1) break;

                        }
                        Console.WriteLine("");
                        Console.WriteLine("Thread data: " + nr);
                        string[] parts_data = data.Split(' ');
                        
                        

                       
                        if (parts_data[1]=="/" ) parts_data[1] = "html_files/index.html";
                        if ( parts_data[1].Substring(0, 1)=="/") parts_data[1]= parts_data[1].Substring(1, parts_data[1].Length-1);
                        string path =  parts_data[1];
                        Console.WriteLine("Sending: " + parts_data[1] +" on path "+path);
                        if (File.Exists(path))
                        {

                            string return_header = "HTTP/1.1 200 Everything is Fine\n Server: C#_httpServer\nContent-Type: text/html; charset: UTF-8\n\n";

                            string return_string = return_header;

                            byte[] return_Data = Encoding.ASCII.GetBytes(return_string);
                            client.SendTo(return_Data, client.RemoteEndPoint);

                            client.SendFile(path);
                            client.Close();
                        }
                        else if (Directory.Exists(path))
                        {
                            string return_body = "<html> <h1> List Of Files in Directory </h1> </endl>";
                            string[] fileEntries = Directory.GetFiles(path);
                            foreach(string file in fileEntries)
                            {
                                string file_var=file.Replace("html_files/", "");
                                return_body = return_body + "<p><a href=" +file_var + ">" + file_var + "</a></p> </endl>";
                            }
                            return_body = return_body + "</html>";
                            string return_header = "HTTP/1.1 200 Everything is Fine\n Server: C#_httpServer\nContent-Type: text/html; charset: UTF-8\n\n";

                            string return_string = return_header + return_body;

                            byte[] return_Data = Encoding.ASCII.GetBytes(return_string);
                            client.SendTo(return_Data, client.RemoteEndPoint);

                            client.SendFile(path);
                            client.Close();
                        }
                        else
                        {
                            string return_header = "HTTP/1.1 404 File Not Found\n Server: C#_httpServer\nContent-Type: text/html; charset: UTF-8\n\n";
                            string return_string = return_header + "404 File Not Found";
                            byte[] return_Data = Encoding.ASCII.GetBytes(return_string);

                            
                            client.SendTo(return_Data, client.RemoteEndPoint);
                            client.Close();
                            throw new FileNotFoundException("file " + path + " not found");
                        }

                    }
                }

            }
            catch (FileNotFoundException exc)
            {
                Console.WriteLine("Error: " + exc.Message);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);

                ;

            }
        }

    }
}
