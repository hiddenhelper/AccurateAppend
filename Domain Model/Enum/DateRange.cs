using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AccurateAppend.Core;

namespace DomainModel.Enum
{
    public enum DateRange
    {
        [Display, Description("Last 24 Hours")]
        Last24Hours = 1,
        [Display, Description("Today")]
        Today = 2,
        [Display, Description("Yesterday")]
        Yesterday = 3,
        [Display, Description("Last 7 Days")]
        Last7Days = 4,
        [Display, Description("Last Month")]
        LastMonth = 6,
        [Display, Description("Last 30 Days")]
        Last30Days = 7,
        [Display, Description("Last 60 Days")]
        Last60Days = 8,
        [Display, Description("Last 90 Days")]
        Last90Days = 9,
        [Display, Description("Current Month")]
        CurrentMonth = 10,
        [Display, Description("Previous Month")]
        PreviousToLastMonth = 11,
        [Display, Description("All")]
        All = 12,
        [Display, Description("Custom date range")]
        Custom = 13,
        [Display, Description("Last 60 Minutes")]
        Last60Minutes = 14,
        [Description("Last 365 Days")]
        LastYear = 15
    }

    public static class DateRangeExtensions
    {
        public static DateSpan CalculatePeriod(DateTime fromDate, DateRange dateRange)
        {
            DateTime floor;

            switch (dateRange)
            {
                case DateRange.CurrentMonth:
                    floor = fromDate.ToFirstOfMonth();
                    break;
                case DateRange.Last24Hours:
                    floor = fromDate.AddDays(-1);
                    break;
                case DateRange.Last30Days:
                    floor = fromDate.AddDays(-30);
                    break;
                case DateRange.Today:
                    floor = fromDate.ToStartOfDay();
                    break;
                case DateRange.Yesterday:
                    floor = fromDate.AddDays(-1).ToStartOfDay();
                    break;
                case DateRange.Last7Days:
                    floor = fromDate.AddDays(-1);
                    break;
                case DateRange.LastMonth:
                    floor = fromDate.ToFirstOfMonth().AddMonths(-1);
                    break;
                case DateRange.Last60Days:
                    floor = fromDate.AddDays(-60);
                    break;
                case DateRange.Last90Days:
                    floor = fromDate.AddDays(-90);
                    break;
                case DateRange.PreviousToLastMonth:
                    floor = fromDate.ToFirstOfMonth().AddMonths(-2);
                    break;
                case DateRange.Last60Minutes:
                    floor = fromDate.AddMinutes(-60);
                    break;
                case DateRange.LastYear:
                    floor = fromDate.AddYears(-1);
                    break;
                case DateRange.All:
                    floor = DateTime.MinValue;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(dateRange), dateRange, $"{nameof(dateRange)} must be a values calculatable from now");
            }

            return new DateSpan(floor, fromDate);
        }
    }
}
