using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using GitCommitArtCli.Models;

using LibGit2Sharp;

namespace GitCommitArtCli.Services {
    public class TempLocalGitService : IDisposable, ITempLocalGitService {
        private const string ART_COMMIT_TEXT = "ArtCommit";
        private static int _commits = 0;

        private readonly GitConfig _gitConfig;
        private readonly Identity _identity;
        private readonly Repository _repository;

        private readonly string _localGitFolder;

        public TempLocalGitService(GitConfig gitConfig) {
            var localRepoName = Guid.NewGuid().ToString();
            _localGitFolder = Path.Combine("Repos", localRepoName);
            _gitConfig = gitConfig;

            Log.Info($"Building Temp Repo: {localRepoName}\nCommiting with email: {gitConfig.Email}\n" +
                $"Art Branch: {gitConfig.ArtBranch}\nReset Branch: {gitConfig.MainBranch}");

            _identity = new Identity(gitConfig.Username, gitConfig.Email);

            Repository.Clone(gitConfig.RemoteUrl, _localGitFolder);

            _repository = new Repository(_localGitFolder, new RepositoryOptions {
                Identity = _identity
            });

            var drawBranch = _repository.Branches[gitConfig.ArtBranch];
            Commands.Checkout(_repository, drawBranch);
        }

        public void BatchPushCommit(DateTimeOffset[] dates, int times, int maxBatch = 150) {
            // HashSet<DateTimeOffset> shadedPixels = new();
            foreach (var date in dates) {
                var commitTimes = 40;

                //if (shadedPixels.Add(date)) {
                //    commitTimes -= UpdateExistingCommits(shadedPixels, date, times);
                //    Log.Debug($"Updated {40 - commitTimes} Existing Pixels!");
                //}

                BatchPushCommit(date, commitTimes, maxBatch);
            }
            PushNetwork();
        }

        public void BatchPushCommit(DateTimeOffset date, int times, int maxBatch = 150) {
            var signature = new Signature(_identity, date);

            for (var x = 0; x < times; x++) {
                _repository.Commit(string.Empty, signature, signature, new CommitOptions() {
                    AllowEmptyCommit = true
                });

                if (_commits >= maxBatch) {
                    PushNetwork();
                }
                _commits++;
            }

            Console.Title = $"Stored Commits: {_commits}";
        }

        //public int UpdateExistingCommits(HashSet<DateTimeOffset> shadedPixels, DateTimeOffset updateDate, int max) {
        //    PullNetwork();
        //    var allCommits = _repository.Commits.Where(i => !shadedPixels.Contains(i.Author.When) && i.Parents.Any()).Take(max);
        //    var signature = new Signature(_identity, updateDate);

        //    try {
        //        _repository.Refs.RewriteHistory(new RewriteHistoryOptions {
        //            OnError = (error) => { },
        //            OnSucceeding = () => { },
        //            CommitHeaderRewriter = (commit) => CommitRewriteInfo.From(commit, signature, signature),
        //        }, allCommits);
        //    }
        //    catch(Exception exception) {
        //        Log.Error(exception.ToString());
        //        return 0;
        //    }

        //    return allCommits.Count();
        //}

        public void PushNetwork() {
            if (_commits > 0) {
                Log.Info($"Pushing: {_commits} Commits.");
                Console.Title = $"Pushing: {_commits} Commits.";
            }

            _repository.Network.Push(_repository.Head, new PushOptions {
                CredentialsProvider = (url, user, cred) =>
                new UsernamePasswordCredentials() {
                    Username = _gitConfig.Username,
                    Password = _gitConfig.AccessToken
                },
            });
            _commits = 0;
        }

        public void PullNetwork() {
            _repository.Network.Fetch("origin", new List<string> { _repository.Head.CanonicalName });
        }

        public void Dispose() {
            // TODO: fix files not deleting.
            Log.Info($"Disposing Temp Repo: {_localGitFolder}");
            ForceDirectory.ForceDeleteFiles(_localGitFolder);
        }
    }
}
