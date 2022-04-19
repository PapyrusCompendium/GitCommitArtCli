using System;
using System.Linq;
using System.Reflection;

using GitCommitArtCli.Models;
using GitCommitArtCli.Services;

using Microsoft.Extensions.Configuration;


namespace GitCommitArtCli {
    public class Program {
        private static GitConfig _gitConfig;
        private static TextRenderingConfig _textRenderingConfig;
        public static void Main(string[] args) {
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonFile("appsettings.json", false)
               .AddJsonFile("appsettings.development.json", true)
               .AddUserSecrets(Assembly.GetAssembly(typeof(Program)));

            var configuration = configBuilder.Build();
            _gitConfig = new GitConfig(configuration);
            _textRenderingConfig = new TextRenderingConfig(configuration);
            Console.WriteLine($"Loaded config for {_gitConfig.Username}, and font {_textRenderingConfig.FontImage}");

            CleanCanvas();
            DrawTextArt();
        }

        private static void DrawTextArt() {
            var textRenderService = new TextRendering(_textRenderingConfig);
            using var tempGitClone = new TempLocalGitService(_gitConfig);

            Console.Write("Text: ");
            var artText = Console.ReadLine();
            var commitPixelString = textRenderService.RenderPixelLetters(artText);

            var commitShading = 10;
            foreach (var letter in commitPixelString) {
                foreach (var pixel in letter.Pixels) {
                    tempGitClone.Commit(pixel.Date, commitShading);
                }
                tempGitClone.PushNetwork();
                commitShading += 5;
            }

        }

        private static void CleanCanvas() {
            Console.WriteLine("Cleaning Art Canvas...");
            var githubApi = new GithubApiService(_gitConfig);

            githubApi.HardResetArtBranch().Wait();
            Console.WriteLine("Canvas is clear!");
        }
    }
}
