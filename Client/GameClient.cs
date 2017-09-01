using Client.Models;
using Common.Extensions;
using Common.Models;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Client
{
    public class GameClient : IDisposable
    {
        private Int32 _counter;
        private TcpClient _client;
        private readonly ManualResetEventSlim _workingMres;
        private readonly ManualResetEventSlim _connectedMres;
        private readonly ClientConfig _config;


        public GameClient(ClientConfig config)
        {
            _config = config;
            _workingMres = new ManualResetEventSlim(false);
            _connectedMres = new ManualResetEventSlim(false);

            ThreadPool.QueueUserWorkItem(ConnectionThread);
            ThreadPool.QueueUserWorkItem(ReceivingThread);
            ThreadPool.QueueUserWorkItem(SendingThread);
        }


        public delegate void UserNotificationAvaliableEventHandler(object sender, UserNotificationAvaliableEventArgs e);
        public event UserNotificationAvaliableEventHandler UserNotificationAvaliable = (sender, args) => { };


        // THREADS ////////////////////////////////////////////////////////////////////////////////
        private void ConnectionThread(Object state)
        {
            while (true)
            {
                _workingMres.Wait();

                if (_client == null || !_client.Connected)
                {
                    Connect();
                }

                Thread.Sleep(10);
            }
        }
        private void ReceivingThread(Object state)
        {
            while (true)
            {
                try
                {
                    _connectedMres.Wait();

                    using (var stream = _client.GetStream())
                    {
                        while (true)
                        {
                            _workingMres.Wait();
                            ReceiveMessage(stream);
                        }
                    }
                }
                catch (SocketException ex)
                {

                }
                catch (IOException ex)
                {

                }
                catch (ArgumentException ex)
                {

                }
                catch (Exception ex)
                {
                    UserNotificationAvaliable.Invoke(this, new UserNotificationAvaliableEventArgs(ex.Message));
                    throw;
                }
                finally
                {
                    _connectedMres.Reset();
                }
            }
        }
        private void SendingThread(Object state)
        {
            while (true)
            {
                try
                {
                    _connectedMres.Wait();

                    using (var stream = _client.GetStream())
                    {
                        while (true)
                        {
                            _workingMres.Wait();
                            SendMessage(stream);
                            Thread.Sleep(500);
                        }
                    }
                }
                catch (SocketException ex)
                {

                }
                catch (IOException ex)
                {

                }
                catch (ArgumentException ex)
                {

                }
                catch (Exception ex)
                {
                    UserNotificationAvaliable.Invoke(this, new UserNotificationAvaliableEventArgs(ex.Message));
                    throw;
                }
                finally
                {
                    _connectedMres.Reset();
                }
            }
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void Start()
        {
            _workingMres.Set();
        }
        public void Stop()
        {
            _workingMres.Reset();
        }
        private void Connect()
        {
            try
            {
                _client = new TcpClient(_config.Host, _config.Port);
                SendConnectionRequest(_client.GetStream());
                UserNotificationAvaliable.Invoke(this, new UserNotificationAvaliableEventArgs("Connected..."));
                _connectedMres.Set();
            }
            catch (SocketException ex)
            {
                UserNotificationAvaliable.Invoke(this, new UserNotificationAvaliableEventArgs("Server unavalible. Retrying to connect."));
                Thread.Sleep(_config.ReconnectDelay);
            }
            catch (IOException ex)
            {
                UserNotificationAvaliable.Invoke(this, new UserNotificationAvaliableEventArgs("Server unavalible. Retrying to connect."));
                Thread.Sleep(_config.ReconnectDelay);
            }
            catch (Exception ex)
            {
                UserNotificationAvaliable.Invoke(this, new UserNotificationAvaliableEventArgs(ex.Message));
                throw;
            }
        }
        private void SendConnectionRequest(NetworkStream stream)
        {
            stream.WriteObject(new ConnectionRequest()
            {
                RoomId = _config.RoomId,
                ClientId = _config.ClientId
            });
        }
        private void SendMessage(NetworkStream stream)
        {
            stream.WriteObject(new Message()
            {
                Body = $"Message number: {_counter++}"
            });
        }
        private void ReceiveMessage(NetworkStream stream)
        {
            var message = stream.ReadObject<Message>().Body;
            Console.WriteLine($"Client id: {_config.ClientId}, Room id: {_config.RoomId}: {message}");
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            _client?.Close();
            _workingMres?.Dispose();
            _connectedMres?.Dispose();
        }
    }
}