using System;
using System.Collections.Generic;
using System.Text;

namespace WiseDomain
{
    public static class TimeFormatter
    {
        static readonly string[] years = { "год", "года", "лет" };
        static readonly string[] months = { "месяц", "месяца", "месяцев" };
        static readonly string[] weeks = { "неделя", "недели", "недель" };
        static readonly string[] days = { "день", "дня", "дней" };
        static readonly string[] hours = { "час", "часа", "часов" };
        static readonly string[] minutes = { "минута", "минуты", "минут" };
        static readonly string[] seconds = { "секунда", "секунды", "секунд" };


        public static string GetString(TimeSpan time)
        {
            if (time.TotalDays > 365)
                return $"{time.Days / 365} {GetCase(time.Days / 365, years)}";
            if (time.TotalDays > 30)
                return $"{time.Days / 30} {GetCase(time.Days / 30, months)}";
            if (time.TotalDays > 7)
                return $"{time.Days / 7} {GetCase(time.Days / 7, weeks)}";
            if (time.TotalDays > 1)
                return $"{time.Days} {GetCase((int)time.TotalDays, days)}";
            if (time.TotalHours > 1)
                return $"{time.Hours} {GetCase((int)time.TotalHours, hours)}";
            if (time.TotalMinutes > 1)
                return $"{time.Minutes} {GetCase((int)time.TotalMinutes, minutes)}";
            
            return $"{time.Seconds} {GetCase((int)time.TotalSeconds, seconds)}";
        }

        static string GetCase(int value, string[] options)
        {
            value = Math.Abs(value) % 100;

            if (value > 10 && value < 15)
                return options[2];

            value %= 10;
            if (value == 1) return options[0];
            if (value > 1 && value < 5) return options[1];
            return options[2];
        }
    }
}
