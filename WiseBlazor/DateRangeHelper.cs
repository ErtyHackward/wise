using BlazorDateRangePicker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WiseBlazor
{
    public static class DateRangeHelper
    {
        public static Dictionary<string, DateRange> GetDefaultRanges()
        {
            return new Dictionary<string, DateRange>() {
                { "Сегодня", new DateRange() {
                        Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
                        End = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddTicks(-1)
                    }
                },
                { "Последние 7 дней", new DateRange() {
                        Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-7),
                        End = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(1).AddTicks(-1)
                    }
                },
                { "С начала месяца", new DateRange() {
                        Start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                        End = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddTicks(-1)
                    }
                },
            };
        }
    }
}
