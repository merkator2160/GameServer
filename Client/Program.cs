using Common.Extensions;
using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private const String HOST = "127.0.0.1";
        private const Int32 PORT = 8888;
        private const Int32 REPEAT_REQUEST_TIME = 100;
        private const Int32 RECONNECT_TO_SERVER_TIMEOUT = 3000;
        private const Int32 SEND_RECEIVE_OPERATIONS_TIMEOUT = 3000;

        private static TcpClient _client;
        private static CancellationTokenSource _cancelTokenSource;


        static void Main(String[] args)
        {
            try
            {
                using (_cancelTokenSource = new CancellationTokenSource())
                {
                    var token = _cancelTokenSource.Token;
                    RunClient(token);

                    Console.ReadKey();
                    _cancelTokenSource.Cancel();
                }
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine("Something is broken:");
                Console.WriteLine(ex.Message);
                Console.ReadKey();
            }
            finally
            {
                _client?.Close();
                _cancelTokenSource?.Dispose();
            }
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        private static void RunClient(CancellationToken token)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {
                        var stream = Reconnect();
                        while (true)
                        {
                            if (token.IsCancellationRequested)
                            {
                                _client.Close();
                                return;
                            }

                            SendRequest(stream);
                            ReceiveResponce(stream);

                            Thread.Sleep(REPEAT_REQUEST_TIME);
                        }
                    }
                    catch (SocketException ex)
                    {
                        Console.Clear();
                        Console.WriteLine("Server unavalible. Retry to connect.");
                        Thread.Sleep(RECONNECT_TO_SERVER_TIMEOUT);
                    }
                }
            }, token);
        }
        private static NetworkStream Reconnect()
        {
            _client = new TcpClient(HOST, PORT)
            {
                SendTimeout = SEND_RECEIVE_OPERATIONS_TIMEOUT,
                ReceiveTimeout = SEND_RECEIVE_OPERATIONS_TIMEOUT
            };
            return _client.GetStream();
        }
        private static void SendRequest(NetworkStream stream)
        {
            stream.WriteString("Hi!");
        }
        private static void ReceiveResponce(NetworkStream stream)
        {
            Console.WriteLine(stream.ReadString());
        }
    }
}
