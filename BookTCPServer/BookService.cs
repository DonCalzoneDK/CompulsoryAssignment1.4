using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using BookClassLibrary;

namespace BookServerTCP
{
    class BookService
    {

        private static List<Book> bList = new List<Book>()
        {
            new Book {Author = "Kurt Vonnegut", PageNumber = 302, Isbn13 = "0-385-28089-0", Title = "Breakfast of Champions"},
            new Book {Author = "Kurt Vonnegut", PageNumber = 215, Isbn13 = "0-385-31208-3", Title = "Slaughterhouse-Five"},
            new Book {Author = "Kurt Vonnegut", PageNumber = 304, Isbn13 = "0-385-33348-X", Title = "Cat's Cradle"},
            new Book {Author = "Frank Herbert", PageNumber = 412, Isbn13 = "978-044117271", Title = "Dune"},
            new Book {Author = "George Orwell", PageNumber = 328, Isbn13 = "978-045152493", Title = "1984"},
            new Book {Author = "Johnathan Franzen", PageNumber = 576, Isbn13 = "0-374-15846-0", Title = "Freedom"}
        };

        private TcpClient connectionSocket;
        private TcpListener serverSocket;


        public BookService(TcpClient connectionSocket)
        {
            this.connectionSocket = connectionSocket;
        }

        public BookService(ref TcpClient connectionSocket, ref TcpListener serverSocket)
        {
            this.connectionSocket = connectionSocket;
            this.serverSocket = serverSocket;
        }
        internal void DoIt()
        {
            Stream ns = connectionSocket.GetStream();
            StreamReader sr = new StreamReader(ns);
            StreamWriter sw = new StreamWriter(new BufferedStream(ns));
            sw.AutoFlush = true;
            sw.Write("Please make all request in capital letters\r\n");
            sw.Write("Types of Requests: GET - GETALL - SAVE\r\n");
            sw.Write("Please Write Full Request in Capital Letters\r\n");
            sw.Write("Enter the command STOP if you want to stop the application!\r\n");
            string message = sr.ReadLine();
            string answer, first;
            while (message != null && message != "")
            {
                Console.WriteLine("Client: " + message);
                string[] list = message.Split(' ');
                first = list[0].ToUpper();
                if (first.Equals("STOP"))
                {
                    Console.WriteLine("Wants to stop");
                    ns.Close();
                    connectionSocket.Close();
                    while (serverSocket.Pending())
                        Thread.Sleep(100);
                    Console.WriteLine("Shutdown says; Server is stopped");

                    serverSocket.Stop();
                    break;
                }

                if (first.Equals("GET") && list.Length == 2)
                {
                    string stringJson = JsonSerializer.Serialize(GetBook(list[1]));
                    sw.Write(stringJson);
                }
                if (first.Equals("SAVE") && list.Length == 2)
                {

                    SaveBook(JsonSerializer.Deserialize<Book>(list[1]));

                }

                if (first.Equals("GETALL") && list.Length == 1)
                {
                    string stringJson = JsonSerializer.Serialize(GetAllBooks());
                    sw.Write(stringJson);
                    break;

                }
                else
                {
                    answer = message.ToUpper();
                    sw.WriteLine(answer);
                    message = sr.ReadLine();
                }
            }

            sw.Write("\r\n");
            sw.Flush();
            sw.BaseStream.Flush();
            sw.Close();
            ns.Close();
            connectionSocket.Close();
        }

        private Book GetBook(string isbn13)
        {
            return bList.FirstOrDefault(b => b.Isbn13 == isbn13);
        }

        private List<Book> GetAllBooks()
        {
            return bList;
        }

        private void SaveBook(Book newBook)
        {
            bList.Add(newBook);
        }

    }
}
