using Microsoft.Extensions.Configuration;

namespace GitCommitArtCli.Models {
    public class TextRenderingConfig {
        public TextRenderingConfig(IConfiguration configuration) {
            configuration.Bind("TextRenderingConfig", this);
        }

        public TextRenderingConfig() {

        }
        public string FontImage { get; set; }
        public string FontTextOrder { get; set; }
    }
}
