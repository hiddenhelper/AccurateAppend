using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Web;
using AccurateAppend.Core;

// ReSharper disable CheckNamespace
namespace AccurateAppend.Websites
    // ReSharper restore CheckNamespace
{
    public static class DateTimeExtensions
    {
        public static string ToStandardDisplay(this DateTime date)
        {
            date = date.ToUserLocal();

            return date.ToString(CultureInfo.CurrentCulture);
        }

        public static string ToStandardTooltip(this DateTime date)
        {
            date = date.ToUserLocal();

            var result = date.ToString("f", CultureInfo.CurrentCulture);

            return result;
        }

        /// <summary>
        /// Provides the ability to perform conversion of a supplied <see cref="DateTime"/>
        /// into <see cref="Core.DateTimeExtensions.Coerce(Nullable{DateTime})">coerced</see> local 
        /// time and adjusted by the client local timezone (determined during logon).
        /// </summary>
        /// <remarks>
        /// Output will ALWAYS be in <see cref="DateTimeKind">Local</see>.
        /// Multiple chaining of this method is not guarded against.
        /// </remarks>
        /// <param name="date">The <see cref="DateTime"/> to convert.</param>
        /// <returns>A new <see cref="DateTime"/> in Local.</returns>
        public static DateTime ToUserLocal(this DateTime date)
        {
            //Contract.Ensures(Contract.Result<DateTime>().Kind == DateTimeKind.Local);

            date = date.Coerce();

            if (HttpContext.Current == null) return date;

            var cookie = HttpContext.Current.Request.Cookies["GMT Offset"];
            var offset = 0;
            if (cookie != null && cookie.Value != null) Int32.TryParse(cookie.Value, out offset);

            date = offset != 0 
                ? date.AddHours(offset) 
                : date.ToLocalTime();

            date = new DateTime(date.Ticks, DateTimeKind.Local);

            return date;
        }

        /// <summary>
        /// Provides the ability to perform conversion of a supplied <see cref="DateTime"/>
        /// into server local time based on the client local timezone (determined during logon).
        /// </summary>
        /// <remarks>
        /// Output will ALWAYS be in <see cref="DateTimeKind">Local</see>.
        /// Multiple chaining of this method is not guarded against.
        /// </remarks>
        /// <param name="date">The <see cref="DateTime"/> to convert.</param>
        /// <returns>A new <see cref="DateTime"/> in Local.</returns>
        public static DateTime FromUserLocal(this DateTime date)
        {
            Contract.Ensures(Contract.Result<DateTime>().Kind == DateTimeKind.Local);
          
            if (HttpContext.Current == null) return date;

            var offset = 0;
            var cookie = HttpContext.Current.Request.Cookies["GMT Offset"];
            if (cookie != null && cookie.Value != null) Int32.TryParse(cookie.Value, out offset);

            var localOffset = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalHours;
            var adjustment = localOffset - offset;
            date = offset != 0
                ? date.AddHours(adjustment*-1) 
                : date.ToLocalTime();

            date = new DateTime(date.Ticks, DateTimeKind.Local);

            return date;
        }
    }
}