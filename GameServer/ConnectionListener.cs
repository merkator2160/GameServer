using GameServer.Models;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;

namespace GameServer
{
    internal class ConnectionListener : IDisposable
    {
        private readonly TcpListener _tcpListener;


        public ConnectionListener(Int32 port)
        {
            _tcpListener = TcpListener.Create(port);
            _tcpListener.Start();
            ThreadPool.QueueUserWorkItem(AcceptNewClientThread);
        }


        public delegate void NewClientAvaliableEventHandler(object sender, NewClientAvaliableEventArgs e);
        public event NewClientAvaliableEventHandler NewClientAvaliable = (sender, args) => { };


        // THREADS ////////////////////////////////////////////////////////////////////////////////
        private void AcceptNewClientThread(Object state)
        {
            while (true)
            {
                try
                {
                    NewClientAvaliable.Invoke(this, new NewClientAvaliableEventArgs(_tcpListener.AcceptTcpClient()));
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
            _tcpListener?.Stop();
        }
    }
}