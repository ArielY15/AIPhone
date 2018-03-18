using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
        

        public OcrRec()
        {
            InitializeComponent();
            ocrRecMananger = new OCRRecMananger();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cameraIds = webCameraControl.GetVideoCaptureDevices();
            var cameraIdEnum = cameraIds.GetEnumerator();
            while (cameraIdEnum.Current == null)
                cameraIdEnum.MoveNext();
            webCameraControl.StartCapture(cameraIdEnum.Current);

            var detected = false;
            do
            {

                var image = webCameraControl.GetCurrentImage();
                var str = await ocrRecMananger.UploadAndDetectText(image);
                ocrResult.Content = str;

                detected = str.ToLower() == "microsoft"; // TODO: set correct result

                await Task.Delay(1000);
            } while (!detected);
        }
    }
}
