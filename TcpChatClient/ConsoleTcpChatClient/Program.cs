using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleTcpChatClient
{
    class Program
    {
        static string _userName;
        private const string Host = "127.0.0.1";
        private const int Port = 8888;
        static TcpClient _client;
        static NetworkStream _stream;

        static void Main(string[] args)
        {
            Console.Write("Введите свое имя: ");
            _userName = Console.ReadLine();
            _client = new TcpClient();
            try
            {
                _client.Connect(Host, Port); //подключение клиента
                _stream = _client.GetStream(); // получаем поток

                string message = _userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                _stream.Write(data, 0, data.Length);

                // запускаем новый поток для получения данных
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); //старт потока
                Console.WriteLine("Добро пожаловать, {0}", _userName);
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }

        // отправка сообщений
        private static void SendMessage()
        {
            Console.WriteLine("Введите сообщение: ");

            while (true)
            {
                string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                _stream.Write(data, 0, data.Length);
            }
        }

        // получение сообщений
        static void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes;
                    do
                    {
                        bytes = _stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    } while (_stream.DataAvailable);

                    string message = builder.ToString();
                    Console.WriteLine(message); //вывод сообщения
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!"); //соединение было прервано
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        static void Disconnect()
        {
            _stream?.Close(); //отключение потока
            _client?.Close(); //отключение клиента
            Environment.Exit(0); //завершение процесса
        }
    }
}
