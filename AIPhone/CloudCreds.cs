using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIPhone
{
    class CloudCreds
    {
        private static CloudCreds instance = null;

        public readonly string FaceAPIKey = "d9d7d017ef4c427ebb0b3aa881a5447b";
        public readonly string FaceAPIUrl = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/";

        public readonly string SpeechAPIKey = "937eadf7077443c98a1d00f4c788c9ea";

        public readonly string VisionAPIKey = "3268f68e2f73460d82982b5086991565";
        public readonly string VisionAPIUrl = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0";


        protected CloudCreds() { }

        public static CloudCreds GetInstance()
        {
            if(instance == null)
            {
                instance = new CloudCreds();
            }
            return instance;
        }
    }
}
