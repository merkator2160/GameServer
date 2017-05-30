using ProtoBuf;
using System.Net.Sockets;
using System.Text;

namespace Common.Extensions
{
    public static class NetworkStreamExtensions
    {
        public static string ReadString(this NetworkStream stream)
        {
            var buffer = new byte[256];
            var stringBuilder = new StringBuilder();

            do
            {
                int bytes = stream.Read(buffer, 0, buffer.Length);
                stringBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
            }
            while(stream.DataAvailable);

            return stringBuilder.ToString();
        }
        public static void WriteString(this NetworkStream stream, string message)
        {
            var dataOut = Encoding.UTF8.GetBytes(message);
            stream.Write(dataOut, 0, dataOut.Length);
        }
        public static T Read<T>(this NetworkStream stream)
        {
            return Serializer.Deserialize<T>(stream);
        }
        public static void Write<T>(this NetworkStream stream, T instance)
        {
            Serializer.Serialize(stream, instance);
        }
    }
}