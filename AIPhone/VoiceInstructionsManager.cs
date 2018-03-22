using System;
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
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Assets/Sounds/AI1.wav");
            player.Play();
        }

        public void OpenThePhone()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Assets/Sounds/AI2.wav");
            player.PlaySync();
        }

        public void FaceInstruction()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Assets/Sounds/AI3.wav");
            player.Play();
        }

        public void FaceSuccsided()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Assets/Sounds/AI4.wav");
            player.Play();
        }

        public void SpeechInstructions()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Assets/Sounds/AI5.wav");
            player.Play();
        }

        public void OcrInstruction()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Assets/Sounds/AI6.wav");
            player.Play();
        }

        public void finish()
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"Assets/Sounds/AI7.wav");
            player.Play();
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
