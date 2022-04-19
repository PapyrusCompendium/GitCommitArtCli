using System;

namespace GitCommitArtCli.Services {
    public static class GithubCommitCalandar {
        public static DateTimeOffset TopLeftBlock {
            get {
                var lastSunday = GetLastDayOfWeek(DayOfWeek.Sunday, 1);
                return lastSunday.Subtract(TimeSpan.FromDays(51 * 7));
            }
        }

        public static DateTimeOffset SafeTopRightBlock {
            get {
                return GetLastDayOfWeek(DayOfWeek.Sunday, 1);
            }
        }

        public static DateTimeOffset CurrentTopRightBlock {
            get {
                return GetLastDayOfWeek(DayOfWeek.Sunday);
            }
        }

        public static DateTimeOffset GetLastDayOfWeek(DayOfWeek dayOfWeek, int skips = 0) {
            return GetLastDayOfWeek(dayOfWeek, DateTimeOffset.UtcNow, skips);
        }

        public static DateTimeOffset GetLastDayOfWeek(DayOfWeek dayOfWeek, DateTimeOffset fromTime, int skips = 0) {
            while (fromTime.DayOfWeek != dayOfWeek || skips > 0) {
                if (fromTime.DayOfWeek == dayOfWeek) {
                    skips--;
                }
                fromTime = fromTime.Subtract(TimeSpan.FromDays(1));
            }

            return fromTime;
        }
    }
}
