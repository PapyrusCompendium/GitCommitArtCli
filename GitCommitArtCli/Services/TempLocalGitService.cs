using System;
using System.IO;
using System.Linq;

using GitCommitArtCli.Models;

using LibGit2Sharp;

namespace GitCommitArtCli.Services {
    public class TempLocalGitService : IDisposable, ITempLocalGitService {
        private const string ART_COMMIT_TEXT = "ArtCommit";
        private static int _commits = 0;

        private readonly GitConfig _gitConfig;
        private readonly PushOptions _pushOptions;
        private readonly Identity _identity;
        private readonly Repository _repository;

        private readonly string _localGitFolder;

        public TempLocalGitService(GitConfig gitConfig) {
            _gitConfig = gitConfig;
            _localGitFolder = Path.Combine("Repos", Guid.NewGuid().ToString());


            _pushOptions = new PushOptions {
                CredentialsProvider = (_url, _user, _cred) =>
                new UsernamePasswordCredentials() {
                    Username = _gitConfig.Username,
                    Password = _gitConfig.AccessToken
                }
            };

            _identity = new Identity(gitConfig.Username, gitConfig.Email);

            Repository.Clone(gitConfig.RemoteUrl, _localGitFolder);
            _repository = new Repository(_localGitFolder, new RepositoryOptions {
                Identity = _identity
            });

            var drawBranch = _repository.Branches[gitConfig.ArtBranch];
            Commands.Checkout(_repository, drawBranch);
        }

        public void Commit(DateTimeOffset date, int times) {
            for (var x = 0; x < times; x++) {
                _repository.Commit(ART_COMMIT_TEXT, new Signature(_identity, date),
                    new Signature(_identity, date), new CommitOptions() {
                        AllowEmptyCommit = true
                    });

                if (_commits >= 250) {
                    PushNetwork();
                }
                _commits++;
            }

            Console.Title = $"Stored Commits: {_commits}";
        }

        public void PushResetDrawing() {
            var mainBranch = Commands.Checkout(_repository, $"refs/remotes/origin/{_gitConfig.MainBranch}");

            var firstCommit = mainBranch.Commits.First();
            while (firstCommit.Parents.FirstOrDefault() != null) {
                firstCommit = firstCommit.Parents.FirstOrDefault();
            }

            var drawBranch = _repository.Branches[_gitConfig.ArtBranch];
            Commands.Checkout(_repository, drawBranch);

            _repository.Reset(ResetMode.Hard, firstCommit);
            _repository.Network.Push(_repository.Network.Remotes["origin"], $"+refs/heads/{_gitConfig.ArtBranch}", _pushOptions);
        }

        public void PushNetwork() {
            Console.Title = $"Pushing: {_commits} Commits.";
            _repository.Network.Push(_repository.Head, _pushOptions);
            _commits = 0;
        }

        public void Dispose() {
            ForceDirectory.ForceDeleteFiles(_localGitFolder);
        }
    }
}
