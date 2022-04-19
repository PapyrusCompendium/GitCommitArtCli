using System;

using GitCommitArtCli.Services;

namespace GitCommitArtCli.Models {
    public class GithubCommitPixel {
        public DateTimeOffset Date { get; private set; }
        public GithubCommitPixel() {
            Date = GithubCommitCalandar.TopLeftBlock;
        }

        public GithubCommitPixel(DateTimeOffset dateTimeOffset) {
            Date = dateTimeOffset;
        }

        public void ShiftRight(int times) {
            Date = Date.AddDays(7 * times);
        }

        public void ShiftLeft(int times) {
            Date = Date.Subtract(TimeSpan.FromDays(7 * times));
        }

        public void SetHeight(int height) {
            var topBlock = Date.DayOfWeek == DayOfWeek.Sunday
                ? Date
                : GithubCommitCalandar.GetLastDayOfWeek(DayOfWeek.Sunday, Date);

            var dayOfWeek = GetWeekDayByNumber(height);

            while (topBlock.DayOfWeek != dayOfWeek) {
                topBlock = topBlock.AddDays(1);
            }

            Date = topBlock;
        }

        private DayOfWeek GetWeekDayByNumber(int number) {
            return number switch {
                0 => DayOfWeek.Sunday,
                1 => DayOfWeek.Monday,
                2 => DayOfWeek.Tuesday,
                3 => DayOfWeek.Wednesday,
                4 => DayOfWeek.Thursday,
                5 => DayOfWeek.Friday,
                6 => DayOfWeek.Saturday,
                _ => DayOfWeek.Sunday
            };
        }
    }
}
