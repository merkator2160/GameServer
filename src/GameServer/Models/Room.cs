using Common.Extensions;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GameServer.Models
{
    public class Room
    {
        private CancellationTokenSource _cancelTokenSource;


        public Room(Guid id)
        {
            Id = id;
            Participiants = new List<RoomMember>();
        }


        // PROPERTIES /////////////////////////////////////////////////////////////////////////////
        public Guid Id { get; set; }
        public DateTime LastMessageDate { get; set; }
        public List<RoomMember> Participiants { get; set; }


        // FUNCTIONS //////////////////////////////////////////////////////////////////////////////
        private void BeginChatting()
        {
            using(_cancelTokenSource = new CancellationTokenSource())
            {
                var token = _cancelTokenSource.Token;
                Task.Factory.StartNew(() =>
                {
                    while(true)
                    {
                        foreach(var x in Participiants)
                        {
                            var receivedMessage = x.Stream.Read<Message>();
                            SendMessageToOtherParticipiants(receivedMessage.Body);
                        }
                    }
                }, token);
            }
        }
        private void SendMessageToOtherParticipiants(string message)
        {

        }
    }
}