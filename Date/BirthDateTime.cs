using LanguageExt;
using System;

namespace NetExtensions.Core.Date
{
    public class BirthDateTime : DateTimeValue
    {
        protected BirthDateTime(DateTime dateTime) : this(Validate(dateTime))
        {

        }
        protected BirthDateTime(DateTime? dateTime) : this(Validate(dateTime))
        {
        }

        protected static Validation<string, DateTime> Validate(DateTime dateTime)
        {
            if (dateTime.Year < DateTime.Now.AddYears(-100).Year)
            {
                return $"given birth date is too old: {dateTime}";
            }
            if (dateTime.Year > DateTime.Now.Year)
            {
                return $"given birth date is invalid: {dateTime}";
            }
            return dateTime;
        }

        protected static Validation<string, DateTime> Validate(DateTime? dateTime)
        {
            return dateTime.Match(Validate, () => "empty date");
        }

        protected BirthDateTime(Validation<string, DateTime> dateTime) : base(dateTime)
        {
        }

        public new static BirthDateTime Create(DateTime? dateTime)
        {
            return new BirthDateTime(dateTime);
        }

        public new static BirthDateTime Create(string dateTimeString, bool lastCentury = false)
        {
            var value = Convert(dateTimeString, lastCentury);
            return new BirthDateTime(value);
        }

        public static implicit operator BirthDateTime(string value)
        {
            return new BirthDateTime(value);
        }
    }
}
