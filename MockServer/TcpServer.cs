using System;
using System.Net.Sockets;
using System.Net;

namespace MockServer
{
    public class TcpServer
    {
        private TcpListener server;

        public void Work()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Loopback, 6099);
            try
            {
                // Set the TcpListener on port 13000.
                server = new TcpListener(ipEndPoint);

                // Start listening for client requests.
                server.Start();

                // Buffer for reading data
                Byte[] bytes = new Byte[256];

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    // Perform a blocking call to accept requests.
                    // You could also use server.AcceptSocket() here.
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Connected!");

                    // Get a stream object for reading and writing
                    NetworkStream stream = client.GetStream();

                    int i;

                    // Loop to receive all the data sent by the client.
                    while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        Console.WriteLine("Received bytes: {0}", i);

                        // Send back a response.
                        stream.Write(bytes, 0, i);
                        Console.WriteLine("Echoed bytes: {0}", i);
                    }
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                server.Stop();
            }

            Console.WriteLine("\nHit enter to continue...");
            Console.Read();


        }
    }
}