using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;

namespace AIPhone
{
    public class VoiceInstructionsManager
    {
        private static VoiceInstructionsManager instance;
        protected VoiceInstructionsManager(){}

        public bool code = false;
        public double threshold = 10;
        public bool open = false;

        public static VoiceInstructionsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new VoiceInstructionsManager();
                }
                return instance;
            }
        }

        public void EnterCode()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Assets/Sounds/missionAI1.wav");
            
            while (!code)
            {
                player.Play();
                Thread.Sleep(2000);
            }
            
        }

        public void OpenThePhone(WebEye.Controls.Wpf.WebCameraControl cameraControl)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Assets/Sounds/missionAI1.wav");

            while (!open)
            {
                player.Play();
                //Thread.Sleep(10000);
                Bitmap image =  cameraControl.GetCurrentImage();
                double res = getDominantColor(image);
                if (res > threshold)
                    open = true;
            }
            Thread.Sleep(2000);
        }

        public double getDominantColor(Bitmap bmp)
        {
            //Used for tally
            int r = 0;
            int g = 0;
            int b = 0;
            int total = 0;
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color clr = bmp.GetPixel(x, y);

                    r += clr.R;
                    g += clr.G;
                    b += clr.B;

                    total++;
                }
            }
            double avR = Convert.ToDouble(r) / total;
            double avG = Convert.ToDouble(g) / total;
            double avB = Convert.ToDouble(b) / total;
            return (avR + avG + avB) / 3;
        }


    }
}
