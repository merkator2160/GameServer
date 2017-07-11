using System;

namespace Common
{
    [Serializable]
    public struct SerializableGuid : IComparable, IComparable<SerializableGuid>, IEquatable<SerializableGuid>
    {
        private readonly string _value;

        private SerializableGuid(string value)
        {
            _value = value;
        }

        private SerializableGuid(Guid guid)
        {
            _value = guid.ToString();
        }


        // IComparable ////////////////////////////////////////////////////////////////////////////
        public int CompareTo(object value)
        {
            if (value == null)
                return 1;

            if (!(value is SerializableGuid))
                throw new ArgumentException("Must be SerializableGuid");

            var guid = (SerializableGuid) value;
            return guid._value == _value ? 0 : 1;
        }


        // IComparable<SerializableGuid> //////////////////////////////////////////////////////////
        public int CompareTo(SerializableGuid other)
        {
            return other._value == _value ? 0 : 1;
        }


        // IEquatable<SerializableGuid> ///////////////////////////////////////////////////////////
        public bool Equals(SerializableGuid other)
        {
            return _value == other._value;
        }


        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _value != null ? _value.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return _value != null ? new Guid(_value).ToString() : string.Empty;
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