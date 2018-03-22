using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WebEye.Controls.Wpf;

namespace AIPhone
{
    /// <summary>
    /// Interaction logic for OcrRec.xaml
    /// </summary>
    public partial class OcrRec : Page
    {
        private IEnumerable<WebCameraId> cameraIds;
        private OCRRecMananger ocrRecMananger;
        VoiceInstructionsManager voiceInstructionsManager = VoiceInstructionsManager.Instance;

        public OcrRec()
        {
            InitializeComponent();
            ocrRecMananger = new OCRRecMananger();
            voiceInstructionsManager.OcrInstruction();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cameraIds = webCameraControl.GetVideoCaptureDevices();

            foreach (WebCameraId i in cameraIds)
            {
                if (i.Name.Equals("Integrated Webcam"/*Agama V-1325R"*/))
                {
                    webCameraControl.StartCapture(i);
                    break;
                }
            }
            var detected = false;
            do
            {
                var image = webCameraControl.GetCurrentImage();
                var str = await ocrRecMananger.UploadAndDetectText(image);
                ocrResult.Content = str;

                detected = str.Contains("Tomas"); // TODO: set correct result

                await Task.Delay(1000);
            } while (!detected);
            voiceInstructionsManager.finish();
            Thread.Sleep(10000);
            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }
    }
}
