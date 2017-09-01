using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Common.Extensions
{
    public static class NetworkStreamExtensions
    {
        public static String ReadString(this NetworkStream stream)
        {
            using (var stringReader = new StreamReader(stream, Encoding.UTF8))
            {
                return stringReader.ReadToEnd();
            }
        }
        public static void WriteString(this NetworkStream stream, String message)
        {
            using (var stringWriter = new StreamWriter(stream, Encoding.UTF8))
            {
                stringWriter.Write(message);
            }
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