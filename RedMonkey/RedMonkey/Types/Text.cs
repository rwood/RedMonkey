using System;

namespace RedMonkey.Types
{
    public struct Text : IEquatable<Text>
    {
        private readonly string _value;

        public Text(string value)
        {
            if (!IsValid(value))
                throw new ArgumentException(nameof(value));
            _value = value;
        }

        public static implicit operator Text(string value)
        {
            return new Text(value);
        }

        public static implicit operator string(Text text)
        {
            return text._value;
        }

        public static bool operator ==(Text a, Text b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Text a, Text b)
        {
            return !(a == b);
        }

        public bool Equals(Text other)
        {
            return Equals(_value, other._value);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(Text))
                return false;
            return Equals((Text) obj);
        }

        public override int GetHashCode()
        {
            return _value != null ? _value.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return _value ?? string.Empty;
        }

        private static bool IsValid(string value)
        {
            return value != null;
        }

        public static readonly Text Empty = new Text();
    }
}