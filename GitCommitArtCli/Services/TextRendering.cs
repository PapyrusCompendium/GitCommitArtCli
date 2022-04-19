using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using GitCommitArtCli.Models;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace GitCommitArtCli.Services {
    public class TextRendering : ITextRendering {
        private readonly TextRenderingConfig _textRenderingConfig;
        private readonly Image<Rgba32> _fontImage;

        public TextRendering(TextRenderingConfig textRenderingConfig) {
            _textRenderingConfig = textRenderingConfig;
            var buildDirectory = Path.GetDirectoryName(Assembly.GetAssembly(GetType()).Location);
            var fontPath = Path.Combine(buildDirectory, "Fonts", _textRenderingConfig.FontImage);

            if (!File.Exists(fontPath)) {
                throw new Exception($"{_textRenderingConfig.FontImage} Font does not exist!");
            }
            _fontImage = Image.Load<Rgba32>(fontPath);

            var distinctLetters = new string(_textRenderingConfig.FontTextOrder.Distinct().ToArray());
            if (distinctLetters != _textRenderingConfig.FontTextOrder) {
                throw new Exception("Font definition has duplicate characters.");
            }
        }

        public IEnumerable<GithubCommitLetter> RenderPixelLetters(string text) {
            var offset = 0;
            foreach (var letter in text) {
                var fontLetter = LookupLetter(letter);
                if (fontLetter is not null) {
                    if (fontLetter.IsWhiteSpace) {
                        offset += 3;
                        continue;
                    }

                    fontLetter.ShiftRight(offset);
                    offset += fontLetter.Width + 1;
                    yield return fontLetter;
                }
            }
        }

        private GithubCommitLetter LookupLetter(char letter) {
            var letterIndex = _textRenderingConfig.FontTextOrder.IndexOf(letter);
            if (letterIndex < 0) {
                return null;
            }

            var pixelLetter = ExtractLetter(letterIndex);
            return pixelLetter;
        }

        private GithubCommitLetter ExtractLetter(int letterIndex) {
            var letterOffset = letterIndex * 7;

            var row = letterOffset / _fontImage.Width;
            var verticleOffset = row * 9;

            var horizantalOffset = letterOffset % _fontImage.Width;

            var width = 0;
            var xPixels = new HashSet<int>();
            List<GithubCommitPixel> pixels = new();

            for (var x = 0; x < 5; x++) {
                for (var y = 0; y < 7; y++) {
                    var currentPixel = _fontImage[horizantalOffset + x, verticleOffset + y];
                    if (currentPixel.A < 100) {
                        continue;
                    }

                    if (xPixels.Add(x)) {
                        width++;
                    }

                    var chartPixel = new GithubCommitPixel();
                    chartPixel.ShiftRight(x - xPixels.Min(i => i));
                    chartPixel.SetHeight(y);
                    pixels.Add(chartPixel);
                }
            }

            var pixelLetter = new GithubCommitLetter(pixels.ToArray(), width);
            return pixelLetter;
        }
    }
}
