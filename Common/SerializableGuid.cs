using ProtoBuf;
using System;

namespace Common
{
    [Serializable]
    [ProtoContract]
    public struct SerializableGuid : IComparable, IComparable<SerializableGuid>, IEquatable<SerializableGuid>
    {
        [ProtoMember(1)]
        private readonly String _value;

        private SerializableGuid(String value)
        {
            _value = value;
        }
        private SerializableGuid(Guid guid)
        {
            _value = guid.ToString();
        }


        // IComparable ////////////////////////////////////////////////////////////////////////////
        public Int32 CompareTo(Object value)
        {
            if (value == null)
                return 1;

            if (!(value is SerializableGuid))
                throw new ArgumentException("Must be SerializableGuid");

            var guid = (SerializableGuid)value;
            return guid._value == _value ? 0 : 1;
        }


        // IComparable<SerializableGuid> //////////////////////////////////////////////////////////
        public Int32 CompareTo(SerializableGuid other)
        {
            return other._value == _value ? 0 : 1;
        }


        // IEquatable<SerializableGuid> ///////////////////////////////////////////////////////////
        public Boolean Equals(SerializableGuid other)
        {
            return _value == other._value;
        }


        public override Boolean Equals(Object obj)
        {
            return base.Equals(obj);
        }
        public override Int32 GetHashCode()
        {
            return (_value != null ? _value.GetHashCode() : 0);
        }
        public override String ToString()
        {
            return _value != null ? new Guid(_value).ToString() : String.Empty;
        }


        public static implicit operator SerializableGuid(Guid guid)
        {
            return new SerializableGuid(guid);
        }
        public static implicit operator Guid(SerializableGuid serializableGuid)
        {
            return new Guid(serializableGuid._value);
        }
    }
}