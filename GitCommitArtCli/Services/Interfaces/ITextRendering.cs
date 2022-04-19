using System.Collections.Generic;

using GitCommitArtCli.Models;

namespace GitCommitArtCli.Services {
    public interface ITextRendering {
        IEnumerable<GithubCommitLetter> RenderPixelLetters(string text);
    }
}