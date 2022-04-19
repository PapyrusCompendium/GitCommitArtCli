using Microsoft.Extensions.Configuration;

namespace GitCommitArtCli.Models {
    public class GitConfig {
        public GitConfig(IConfiguration configuration) {
            configuration.Bind("GitConfig", this);
        }

        public GitConfig() {

        }

        public string RepoName { get; set; }
        public string ArtBranch { get; set; }
        public string MainBranch { get; set; }
        public string RemoteUrl { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string AccessToken { get; set; }
    }
}
