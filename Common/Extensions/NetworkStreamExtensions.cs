using System;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Common.Extensions
{
    public static class NetworkStreamExtensions
    {
        public static String ReadString(this NetworkStream stream)
        {
            var buffer = new Byte[256];
            var stringBuilder = new StringBuilder();

            do
            {
                var bytes = stream.Read(buffer, 0, buffer.Length);
                stringBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytes));
            }
            while (stream.DataAvailable);

            return stringBuilder.ToString();
        }
        public static void WriteString(this NetworkStream stream, String message)
        {
            var dataOut = Encoding.UTF8.GetBytes(message);
            stream.Write(dataOut, 0, dataOut.Length);
        }
        public static T ReadObject<T>(this NetworkStream stream)
        {
            return (T)new BinaryFormatter().Deserialize(stream);
        }
        public static void WriteObject<T>(this NetworkStream stream, T instance)
        {
            new BinaryFormatter().Serialize(stream, instance);
        }
    }
}