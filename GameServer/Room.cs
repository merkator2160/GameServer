using Common.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace GameServer
{
    internal class Room : IDisposable
    {
        private readonly List<BufferedClient<Message>> _members;


        public Room(Guid id)
        {
            Id = id;
            LastActivityDate = DateTime.UtcNow;
            _members = new List<BufferedClient<Message>>();

            ThreadPool.QueueUserWorkItem(ChattingThread);
        }


        // THREADS ////////////////////////////////////////////////////////////////////////////////
        private void ChattingThread(Object state)
        {
            while (true)
            {
                try
                {
                    while (true)
                    {
                        lock (_members)
                        {
                            if (_members.All(p => p.ReceivedMessageQueue.IsEmpty))
                                break;

                            RemoveDisconnectedMembers();
                            SendMessages();
                        }
                    }

                    Thread.Sleep(10);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    throw;
                }
            }
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public Guid Id { get; set; }
        public Int32 NumberOfMembers
        {
            get
            {
                lock (_members)
                {
                    return _members.Count();
                }
            }
        }
        public DateTime LastActivityDate
        {
            get => DateTime.FromBinary(Interlocked.Read(ref _lastActivityDate));
            set => Interlocked.Exchange(ref _lastActivityDate, value.ToBinary());
        }
        private Int64 _lastActivityDate;


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        public void AddParticipiant(BufferedClient<Message> newMember)
        {
            lock (_members)
                _members.Add(newMember);
        }
        private void RemoveDisconnectedMembers()
        {
            var disconnectedMembers = _members.Where(p => !p.IsConnected).ToArray();
            foreach (var x in disconnectedMembers)
            {
                _members.Remove(x);
                x.Dispose();
            }
        }
        private void SendMessages()
        {
            foreach (var x in _members)
            {
                if (x.ReceivedMessageQueue.IsEmpty)
                    continue;

                if (x.ReceivedMessageQueue.TryDequeue(out Message message))
                {
                    SendMessageToRoommates(message, x);
                    //SendEcho(message, x);

                    LastActivityDate = DateTime.UtcNow;
                }
            }
        }
        private void SendMessageToRoommates(Message message, BufferedClient<Message> current)
        {
            var roommates = _members.Except(new[] { current });
            foreach (var x in roommates)
            {
                x.SendMessageQueue.Enqueue(message);
            }
        }
        private void SendEcho(Message message, BufferedClient<Message> current)
        {
            current.SendMessageQueue.Enqueue(message);
        }


        // IDisposable ////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            lock (_members)
            {
                foreach (var x in _members)
                {
                    x.Dispose();
                }
            }
        }
    }
}