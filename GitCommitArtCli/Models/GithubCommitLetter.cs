namespace GitCommitArtCli.Models {
    public class GithubCommitLetter {
        public GithubCommitPixel[] Pixels { get; set; }
        public int Width { get; }

        public bool IsWhiteSpace {
            get {
                return Pixels.Length == 0;
            }
        }

        public GithubCommitLetter(GithubCommitPixel[] githubChartPixels, int width) {
            Pixels = githubChartPixels;
            Width = width;
        }

        public void ShiftRight(int times) {
            foreach (var pixel in Pixels) {
                pixel.ShiftRight(times);
            }
        }

        public void ShiftLeft(int times) {
            foreach (var pixel in Pixels) {
                pixel.ShiftLeft(times);
            }
        }
    }
}
