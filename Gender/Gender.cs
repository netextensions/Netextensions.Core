using NetExtensions.ValueObject;
using System;

namespace Netextensions.Core.Gender
{
    public class Gender : ValueObject<Gender>
    {
        public enum Values
        {
            N = 0,
            M = 1,
            F = 2
        }

        private readonly Values _value;
        public static string Undefined => Values.N.ToString();
        public static string Male => Values.M.ToString();
        public static string Female => Values.F.ToString();

        private Gender()
        {

        }
        public Gender(string value)
        {
            if (!Enum.IsDefined(typeof(Values), value) || !Enum.TryParse<Values>(value, out var enumValue))
            {
                throw new ArgumentOutOfRangeException($"Gender has invalid value: {value}");
            }
            _value = enumValue;
        }

        public Gender(Values value)
        {
            _value = value;
        }

        protected override bool EqualsCustom(Gender other)
        {
            if (other == null)
            {
                return false;
            }

            return _value == other._value;
        }

        protected override int GetHashCodeCustom()
        {
            return (int)_value;
        }

        public static implicit operator string(Gender gender)
        {
            if (gender == null)
                return Undefined;
            return gender._value.ToString();
        }

        public static implicit operator Gender(string value)
        {
            return new Gender(value);
        }

        public static explicit operator Gender(Values value)
        {
            return new Gender(value);
        }

        public static explicit operator Values(Gender gender)
        {
            return gender == null ? Values.N : gender._value;
        }
    }
}
