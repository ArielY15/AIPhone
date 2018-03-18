using Microsoft.ProjectOxford.Vision;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPhone
{
    class OCRRecMananger
    {
        private readonly VisionServiceClient ocrServiceClient =
            new VisionServiceClient("3268f68e2f73460d82982b5086991565", "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0");

        public async Task<string> UploadAndDetectText(Bitmap image)
        {
            using (var memstream = new MemoryStream())
            {
                image.Save(memstream, ImageFormat.Bmp);
                memstream.Seek(0, SeekOrigin.Begin);
                var result = await ocrServiceClient.RecognizeTextAsync(memstream);

                var text = new StringBuilder();
                foreach (var region in result.Regions)
                {
                    foreach (var line in region.Lines)
                    {
                        foreach (var word in line.Words)
                        {
                            text.Append(word.Text + " ");
                        }
                    }
                }

                return text.ToString().Trim();
            }
        }
    }
}
