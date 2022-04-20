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
            Log.Info($"Loaded config for {_gitConfig.Username}\nLoaded Font {_textRenderingConfig.FontImage}");

            // CleanCanvas();

            DrawArt();
        }

        private static void DrawArt() {
            // tempGitClone.BatchPushCommit(DateTimeOffset.FromUnixTimeSeconds(903102479), 1);

            using var gitRender = new GitRenderer(_gitConfig, _textRenderingConfig);

            gitRender.DrawClock();

            //Console.Write("Text: ");
            //var artText = Console.ReadLine();
            //gitRender.DrawText(artText);

            //Console.ReadLine();
        }
    }
}
