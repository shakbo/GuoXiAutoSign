using System;
using System.Linq;

namespace GuoXiAutoSign.Services
{
    internal class TimeService
    {
        private static readonly TimeSpan[] TargetTimeSpans =
        {
            new TimeSpan(13, 30, 0),
            new TimeSpan(14, 30, 0),
            new TimeSpan(15, 30, 0),
        };

        public static DateTime GetNextTarget()
        {
            var now = DateTime.Now;
            var currentTimeOfDay = now.TimeOfDay;
            var nextTarget = TargetTimeSpans.Where(t => t > currentTimeOfDay).OrderBy(t => t).FirstOrDefault();

            return nextTarget == default ? now.Date.AddDays(1).Add(TargetTimeSpans.First()) : now.Date.Add(nextTarget);
        }
    }
}