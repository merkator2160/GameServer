using Common.Models.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using Server.Models;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    internal class ConnectionListener : IDisposable
    {
        private readonly RootConfig _config;
        private readonly IMessenger _messenger;
        private Boolean _disposed;
        private readonly TcpListener _tcpListener;
        private readonly ManualResetEventSlim _workingMres;


        public ConnectionListener(RootConfig config, IMessenger messenger)
        {
            _config = config;
            _messenger = messenger;
            _workingMres = new ManualResetEventSlim(false);
            _tcpListener = TcpListener.Create(_config.Port);

            ThreadPool.QueueUserWorkItem(AcceptNewClientThread);
        }

        // FUNCTONS ///////////////////////////////////////////////////////////////////////////////
        public void Start()
        {
            _tcpListener.Start();
            _workingMres.Set();
        }
        public void Stop()
        {
            _tcpListener.Stop();
            _workingMres.Reset();
        }


        // THREADS ////////////////////////////////////////////////////////////////////////////////
        private void AcceptNewClientThread(Object state)
        {
            while (!_disposed)
            {
                try
                {
                    _workingMres.Wait();
                    _messenger.Send(new NewClientAvaliableMessage(_tcpListener.AcceptTcpClient()));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                _workingMres?.Dispose();
                _tcpListener?.Stop();
            }
        }
    }
}