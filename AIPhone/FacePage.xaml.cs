using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WebEye.Controls.Wpf;


namespace AIPhone
{
    /// <summary>
    /// Interaction logic for FacePage.xaml
    /// </summary>
    public partial class FacePage : Page
    {
        SerialPort serial;
        VoiceInstructionsManager voiceInstructionsManager;
        FaceRecManager faceRecManager;
        Face[] faces;
        IEnumerable<WebCameraId> cameraIds;
        double resizeFactor;
        string agentName = "007";

        public bool detected = false;


        public FacePage()
        {
            InitializeComponent();
            SetSerial();
            voiceInstructionsManager = VoiceInstructionsManager.Instance;
            video.LoadedBehavior = MediaState.Manual;
            video.Stop();
            faceRecManager = new FaceRecManager();
        }

        private void SetSerial()
        {
            serial = new SerialPort
            {
                PortName = "COM3", //Com Port Name                
                BaudRate = 9600, //COM Port Sp
                Handshake = Handshake.None,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                ReadTimeout = 200,
                WriteTimeout = 50
            };         
            serial.DataReceived += new SerialDataReceivedEventHandler(RecieveSerial);
            serial.Open();

        }

        private void RecieveSerial(object sender, SerialDataReceivedEventArgs e)
        {
            string recieved_data = serial.ReadLine();
            if (recieved_data.Contains("EnterCode"))
            {
                voiceInstructionsManager.EnterCode();
             }
            if (recieved_data.Contains("OpenThePhone"))
            {
                voiceInstructionsManager.OpenThePhone();
            }
        }

    private void RunMission()
        {
                video.Play();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            button.Visibility = Visibility.Collapsed;
            video.Visibility = Visibility.Visible;
            try
            {
                RunMission();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void DrawRectangle(BitmapSource bs)
        {
            if (faces.Length > 0)
            {
                // Prepare to draw rectangles around the faces.
                DrawingVisual visual = new DrawingVisual();
                DrawingContext drawingContext = visual.RenderOpen();
                drawingContext.DrawImage(bs,
                    new Rect(0, 0, bs.Width, bs.Height));
                double dpi = bs.DpiX;
                resizeFactor = 96 / dpi;

                for (int i = 0; i < faces.Length; ++i)
                {
                    Face face = faces[i];

                    // Draw a rectangle on the face.
                    drawingContext.DrawRectangle(
                        System.Windows.Media.Brushes.Transparent,
                        new System.Windows.Media.Pen(System.Windows.Media.Brushes.Azure, 4),
                        new Rect(
                            face.FaceRectangle.Left * resizeFactor,
                            face.FaceRectangle.Top * resizeFactor,
                            face.FaceRectangle.Width * resizeFactor,
                            face.FaceRectangle.Height * resizeFactor
                            )
                    );
                }

                drawingContext.Close();

                // Display the image with the rectangle around the face.
                RenderTargetBitmap faceWithRectBitmap = new RenderTargetBitmap(
                    (int)(bs.PixelWidth * resizeFactor),
                    (int)(bs.PixelHeight * resizeFactor),
                    96,
                    96,
                    PixelFormats.Pbgra32);

                faceWithRectBitmap.Render(visual);
                ImageBrush ib = new ImageBrush(faceWithRectBitmap);
                imageCanvas.Background = ib;
            }
        }

        private void DisplayDescription()
        {
            descriptionGrid.Visibility = Visibility.Visible;
            age.Content = faces[0].FaceAttributes.Age.ToString();
            gender.Content = faces[0].FaceAttributes.Gender.ToString();
            facialHair.Content = faces[0].FaceAttributes.FacialHair.Beard.ToString();
            glasses.Content = faces[0].FaceAttributes.Glasses.ToString();
            anger.Content = faces[0].FaceAttributes.Emotion.Anger.ToString();
            contempt.Content = faces[0].FaceAttributes.Emotion.Contempt.ToString();
            disgust.Content = faces[0].FaceAttributes.Emotion.Disgust.ToString();
            fear.Content = faces[0].FaceAttributes.Emotion.Fear.ToString();
            happiness.Content = faces[0].FaceAttributes.Emotion.Happiness.ToString();
            neutral.Content = faces[0].FaceAttributes.Emotion.Neutral.ToString();
            sadness.Content = faces[0].FaceAttributes.Emotion.Sadness.ToString();
            surprise.Content = faces[0].FaceAttributes.Emotion.Surprise.ToString();
        }

        private async void Video_MediaEnded(object sender, RoutedEventArgs e)
        {
            cameraIds = webCameraControl.GetVideoCaptureDevices();
            foreach(WebCameraId i in cameraIds)
            {
                if(i.Name.Equals("Integrated Webcam"/*"Agama V-1325R"*/))
                {
                    webCameraControl.StartCapture(i);
                    break;
                }
            }
            
            voiceInstructionsManager.FaceInstruction();
            video.Visibility = Visibility.Hidden;
            Center.Visibility = Visibility.Visible;
            Bitmap image;
            do
            {
                image = webCameraControl.GetCurrentImage();
                faces = await faceRecManager.UploadAndDetectFaces(image);
                if (faces.Length > 0)
                {
                    detected = true;
                    BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                        image.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromWidthAndHeight(image.Width, image.Height));
                    imageCanvas.Visibility = Visibility.Visible;
                    webCameraControl.Visibility = Visibility.Hidden;
                    DrawRectangle(bs);
                    DisplayDescription();

                    Guid[] guids = new Guid[faces.Length];
                    int i = 0;
                    foreach (Face face in faces)
                    {
                        guids[i++] = face.FaceId;
                    }


                    if (await faceRecManager.IsGroupTrained())
                    {
                        agentName = await faceRecManager.FindAgentName(guids);
                        
                    }

                    InstructionLabel.Content = "Welcome Agent " + agentName + "!";
                }
                await Task.Delay(1000);
            } while (!detected);

            voiceInstructionsManager.FaceSuccsided();
            await Task.Delay(10000);

            webCameraControl.StopCapture();

            NavigationService navService = NavigationService.GetNavigationService(this);

            SpeechPage nextPage = new SpeechPage();

            navService.Navigate(nextPage);
        }

    }
}
