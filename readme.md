# Technical task

You must develop an echo-server.
Echo-server consists of rooms.
Each room has a identifier.
When you connect to the server, the client sends the room ID to which it connects and the client ID of the connecting client (logging in to the room).
If the room does not exist at the connection time, a new room with the provided ID must be created.
After successful entry into the room, the client with a frequency of times 100ms, begins to send echo messages containing a text.
An echo message from a room is sent to all clients in the room.
If the room has not received echo messages within 1 minute, the room is removed from the server.

The network protocol of interaction between server and client at your discretion (HTTP/TCP/UDP).