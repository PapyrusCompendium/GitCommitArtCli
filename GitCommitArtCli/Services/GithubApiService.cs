using System.Linq;
using System.Threading.Tasks;

using GitCommitArtCli.Models;

using Octokit;

namespace GitCommitArtCli.Services {
    public class GithubApiService {
        private readonly GitConfig _gitConfig;
        private readonly GitHubClient _gitHubClient;

        public GithubApiService(GitConfig gitConfig) {
            _gitConfig = gitConfig;

            _gitHubClient = new GitHubClient(new ProductHeaderValue("GitCommitArtCli")) {
                Credentials = new Credentials(gitConfig.AccessToken)
            };
        }

        public async Task HardResetArtBranch() {
            var artRepo = await _gitHubClient.Repository.Get(_gitConfig.Username, _gitConfig.RepoName);
            var allBranches = await _gitHubClient.Repository.Branch.GetAll(artRepo.Id);
            var mainBranch = allBranches.SingleOrDefault(i => i.Name == _gitConfig.MainBranch);

            await _gitHubClient.Repository.Edit(artRepo.Id, new RepositoryUpdate(artRepo.Name) {
                DefaultBranch = _gitConfig.MainBranch
            });

            await _gitHubClient.Git.Reference.Delete(artRepo.Id, $"refs/heads/{_gitConfig.ArtBranch}");

            var cleanArtBranch = _gitHubClient.Git.Reference.Create(artRepo.Id, new NewReference($"refs/heads/{_gitConfig.ArtBranch}", mainBranch.Commit.Sha));

            await _gitHubClient.Repository.Edit(artRepo.Id, new RepositoryUpdate(artRepo.Name) {
                DefaultBranch = _gitConfig.ArtBranch
            });
        }
    }
}
