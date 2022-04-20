
using System;
using System.Linq;
using System.Threading;

using GitCommitArtCli.Models;

using LibGit2Sharp;

namespace GitCommitArtCli.Services {
    public class GitRenderer : TempLocalGitService, IDisposable {
        private readonly GitConfig _gitConfig;
        private readonly TextRenderingConfig _textRenderingConfig;

        private readonly TextRendering _textRendering;

        public GitRenderer(GitConfig gitConfig, TextRenderingConfig textRenderingConfig) : base(gitConfig) {
            _gitConfig = gitConfig;
            _textRenderingConfig = textRenderingConfig;

            _textRendering = new TextRendering(_textRenderingConfig);
        }


        public void DrawText(string text) {
            var commitPixelString = _textRendering.RenderPixelLetters(text);
            var pixels = commitPixelString.SelectMany(i => i.Pixels);

            BatchPushCommit(pixels.Select(i => i.Date).ToArray(), 20, 500);
        }

        //public void ShadePixel(GithubCommitPixel githubCommitPixel) {
        //    BatchCommit();
        //}

        public void DrawClock(int WaitEveryMins = 5) {
            while (true) {
                var clockTime = DateTime.Now;
                if (clockTime.Minute % WaitEveryMins == 0) {
                    CleanCanvas();

                    var timeString = $"{clockTime:hh:mm} {clockTime:tt}";
                    Log.Info($"Updating Time - {timeString}");
                    DrawText(timeString);

                    clockTime = DateTime.Now;
                }

                var totalWait = TimeSpan.FromMinutes(WaitEveryMins - (clockTime.Minute % WaitEveryMins) - 1);
                totalWait = totalWait.Add(TimeSpan.FromSeconds(60 - clockTime.Second));

                var projectedReleaseTime = clockTime.Add(totalWait);
                Log.Info($"Waiting {totalWait.Minutes} mins and {totalWait.Seconds} seconds" +
                    $" - {projectedReleaseTime:hh:mm} {projectedReleaseTime:tt}");
                Thread.Sleep(totalWait);
            }
        }

        public void CleanCanvas() {
            var githubApi = new GithubApiService(_gitConfig);
            try {
                githubApi.HardResetArtBranch().Wait();
            }
            catch (Exception) {
                Log.Error($"Failed to connect to Api, trying again.");
                CleanCanvas();
            }
            Log.Info("Cleared Canvas!");
        }

    }
}
