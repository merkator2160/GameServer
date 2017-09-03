using Client.Models;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading;

namespace Client
{
    internal class NumberGenerator : IDisposable
    {
        private readonly Int64 _offset;
        private readonly Int64 _delay;
        private readonly IMessenger _messenger;
        private readonly Timer _timer;
        private Boolean _disposed;
        private Int64 _counter;


        public NumberGenerator(Int64 offset, Int64 delay, IMessenger messenger)
        {
            _offset = offset;
            _delay = delay;
            _messenger = messenger;
            _counter = 0;
            _timer = new Timer(TimerCallback);
        }


        // EVENTS /////////////////////////////////////////////////////////////////////////////////
        private void TimerCallback(Object state)
        {
            _messenger.Send(new NumberGeneratedMessage(_counter++));
        }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void Start()
        {
            _timer.Change(_offset, _delay);
        }
        public void Stop()
        {
            _timer.Change(0, Timeout.Infinite);
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;

                _timer?.Dispose();
            }
        }
    }
}