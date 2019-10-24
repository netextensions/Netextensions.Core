using LanguageExt;
using NetExtensions.ValueObject;
using System;
using System.Collections.Generic;

namespace NetExtensions.Core.Date
{
    public class DateTimeValue : ValueObject<DateTimeValue>
    {
        protected readonly Validation<string, DateTime> _value;
        protected const string YyyyMMddhhssmm = "yyyyMMddhhssmm";
        protected const string YyyyMMdd = "yyyyMMdd";
        protected const string YyMMdd = "yyMMdd";
        protected DateTimeValue()
        {
            _value = "empty date";
        }

        protected DateTimeValue(DateTime dateTime)
        {
            _value = dateTime;
        }

        protected DateTimeValue(Validation<string, DateTime> dateTime)
        {
            _value = dateTime;
        }

        public Validation<string, DateTime> Value => _value.Match<Validation<string, DateTime>>(dt => dt.Date, error => error);

        public static DateTimeValue Create(DateTime dateTime)
        {
            return new DateTimeValue(dateTime);
        }

        public static DateTimeValue Create(DateTime? dateTime)
        {
            return dateTime.Match(dt => new DateTimeValue(dt), () => new DateTimeValue());
        }

        public static DateTimeValue Create(string dateTimeString, bool lastCentury = false)
        {
            var value = Convert(dateTimeString, lastCentury);
            return new DateTimeValue(value);
        }

        public static DateTimeValue Create(string dateTimeString, bool lastCentury = false, params string[] formats)
        {
            if (string.IsNullOrEmpty(dateTimeString) || formats == null || formats.Length == 0)
                return new DateTimeValue();

            var stringDate = dateTimeString.Trim();
            var parsed = Parse(stringDate, formats.Where(x => x.Length >= 8)).Match(dt => dt,
                error => ParseWithoutCenturies(stringDate, formats.Where(x => x.Length == 6), lastCentury)
            );
            return new DateTimeValue(parsed);
        }

        public static DateTimeValue Create(string dateTimeString, int fromYearBelongsToPreviousCentury)
        {
            return Create(dateTimeString, fromYearBelongsToPreviousCentury, new[] { YyyyMMddhhssmm, YyyyMMdd, YyMMdd });
        }
        public static DateTimeValue Create(string dateTimeString, int fromYearBelongsToPreviousCentury, string[] formats)
        {
            if (string.IsNullOrEmpty(dateTimeString) || formats == null || formats.Length == 0)
                return new DateTimeValue();

            var stringDate = dateTimeString.Trim();
            var parsed = Parse(stringDate, formats.Where(x => x.Length >= 8)).Match(dt => dt,
                error => ParseWithoutCenturies(stringDate, formats.Where(x => x.Length == 6), fromYearBelongsToPreviousCentury)
            );
            return new DateTimeValue(parsed);
        }
        protected static Validation<string, DateTime> Parse(string stringDate, IEnumerable<string> formats)
        {
            var enumerable = formats.ToArray();
            if (!enumerable.Any())
                return "no format is found";

            if (TryParse(stringDate, enumerable, out var date))
                return date;

            return $"given string is invalid: {stringDate}";
        }
        protected static Validation<string, DateTime> ParseWithoutCenturies(string stringDate, IEnumerable<string> formats, bool lastCentury)
        {
            var enumerable = formats.ToArray();
            if (!enumerable.Any())
                return "no format is found";

            if (!TryParse(stringDate, enumerable, out var lastCenturyDate))
                return $"given string is invalid: {stringDate}";

            if (Math.Floor((decimal)lastCenturyDate.Year / 100) >= Math.Floor((decimal)DateTime.Now.Year / 100))
            {
                return lastCentury ? lastCenturyDate.AddYears(-100) : lastCenturyDate;
            }
            return lastCentury ? lastCenturyDate : lastCenturyDate.AddYears(100);
        }
        protected static Validation<string, DateTime> ParseWithoutCenturies(string stringDate, IEnumerable<string> formats, int fromYearBelongsToPreviousCentury)
        {
            var enumerable = formats.ToArray();
            if (!enumerable.Any())
                return "no format is found";

            var culture = new CultureInfo("en-US");
            culture.Calendar.TwoDigitYearMax = fromYearBelongsToPreviousCentury + 99;
            if (TryParse(stringDate, enumerable, out var parsedFromShortDate, culture))
                return parsedFromShortDate;

            return $"given string is invalid: {stringDate}";
        }

        protected static Validation<string, DateTime> Convert(string dateTimeString, bool lastCentury)
        {
            if (string.IsNullOrEmpty(dateTimeString))
                return "empty date";

            var stringDate = dateTimeString.Trim();
            if (TryParse(stringDate, new[] { YyyyMMddhhssmm, YyyyMMdd }, out var date))
                return date;

            if (!TryParse(stringDate, YyMMdd, out var lastCenturyDate))
                return $"given string is invalid: {dateTimeString}";

            // do not use TwoDigitYearMax!
            if (Math.Floor((decimal)lastCenturyDate.Year / 100) >= Math.Floor((decimal)DateTime.Now.Year / 100))
            {
                return lastCentury ? lastCenturyDate.AddYears(-100) : lastCenturyDate;
            }

            return lastCentury ? lastCenturyDate : lastCenturyDate.AddYears(100);
        }


        protected override bool EqualsCustom(DateTimeValue other)
        {
            if (other == null)
                return false;

            return other.Value == Value;
        }

        protected override int GetHashCodeCustom()
        {
            return Value.GetHashCode();
        }

        protected static bool TryParse(string stringDate, string format, out DateTime result, CultureInfo cultureInfo = null) => TryParse(stringDate, new[] { format }, out result, cultureInfo);
        protected static bool TryParse(string stringDate, string[] formats, out DateTime result, CultureInfo cultureInfo = null) => DateTime.TryParseExact(stringDate, formats, cultureInfo ?? CultureInfo.InvariantCulture, DateTimeStyles.None, out result);

        public static implicit operator DateTimeValue(string value)
        {
            return new DateTimeValue(value);
        }
    }
}
