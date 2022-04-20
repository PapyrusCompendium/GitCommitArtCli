using System.IO;

namespace GitCommitArtCli.Services {
    public static class ForceDirectory {
        public static void ForceDeleteFiles(string directory) {
            foreach (var subDirectory in Directory.EnumerateDirectories(directory)) {
                try {
                    ForceDeleteFiles(subDirectory);
                }
                catch { }
            }

            foreach (var file in Directory.EnumerateFiles(directory)) {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            Directory.Delete(directory, true);
        }
    }
}
