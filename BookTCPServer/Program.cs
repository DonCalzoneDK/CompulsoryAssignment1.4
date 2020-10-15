using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace BookServerTCP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            TcpListener serverSocket = new TcpListener(ip, 4646);
            serverSocket.Start();
            try
            {
                while (true)
                {
                    TcpClient connectionSocket = serverSocket.AcceptTcpClient();
                    Console.WriteLine("Server activated now");
                    //BookService service = new BookService(connectionSocket)
                    BookService service = new BookService(ref connectionSocket, ref serverSocket);

                    //Task.Factory.StartNew(service.DoIt);
                    // or use delegates 
                    Task.Factory.StartNew(() => service.DoIt());

                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (serverSocket != null) serverSocket.Stop();
            }

            serverSocket.Stop();
        }

    }
}