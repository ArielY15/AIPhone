using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;

namespace AIPhone
{
    class FaceRecManager
    {
        private CloudCreds cloudCreds;
        private readonly IFaceServiceClient faceServiceClient;

        private string persongroupid = "aiphone";

        public FaceRecManager()
        {
            cloudCreds = CloudCreds.GetInstance();
            faceServiceClient = new FaceServiceClient(cloudCreds.FaceAPIKey,cloudCreds.FaceAPIUrl);
        }

        //// Returns a string that describes the given face.

        private string FaceDescription(Face face)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Face: ");

            // Add the gender, age, and smile.
            sb.Append(face.FaceAttributes.Gender);
            sb.Append(", ");
            sb.Append(face.FaceAttributes.Age);
            sb.Append(", ");
            sb.Append(String.Format("smile {0:F1}%, ", face.FaceAttributes.Smile * 100));

            // Add the emotions. Display all emotions over 10%.
            sb.Append("Emotion: ");
            EmotionScores emotionScores = face.FaceAttributes.Emotion;
            if (emotionScores.Anger >= 0.1f) sb.Append(String.Format("anger {0:F1}%, ", emotionScores.Anger * 100));
            if (emotionScores.Contempt >= 0.1f) sb.Append(String.Format("contempt {0:F1}%, ", emotionScores.Contempt * 100));
            if (emotionScores.Disgust >= 0.1f) sb.Append(String.Format("disgust {0:F1}%, ", emotionScores.Disgust * 100));
            if (emotionScores.Fear >= 0.1f) sb.Append(String.Format("fear {0:F1}%, ", emotionScores.Fear * 100));
            if (emotionScores.Happiness >= 0.1f) sb.Append(String.Format("happiness {0:F1}%, ", emotionScores.Happiness * 100));
            if (emotionScores.Neutral >= 0.1f) sb.Append(String.Format("neutral {0:F1}%, ", emotionScores.Neutral * 100));
            if (emotionScores.Sadness >= 0.1f) sb.Append(String.Format("sadness {0:F1}%, ", emotionScores.Sadness * 100));
            if (emotionScores.Surprise >= 0.1f) sb.Append(String.Format("surprise {0:F1}%, ", emotionScores.Surprise * 100));

            // Add glasses.
            sb.Append(face.FaceAttributes.Glasses);
            sb.Append(", ");

            // Add hair.
            sb.Append("Hair: ");

            // Display baldness confidence if over 1%.
            if (face.FaceAttributes.Hair.Bald >= 0.01f)
                sb.Append(String.Format("bald {0:F1}% ", face.FaceAttributes.Hair.Bald * 100));

            // Display all hair color attributes over 10%.
            HairColor[] hairColors = face.FaceAttributes.Hair.HairColor;
            foreach (HairColor hairColor in hairColors)
            {
                if (hairColor.Confidence >= 0.1f)
                {
                    sb.Append(hairColor.Color.ToString());
                    sb.Append(String.Format(" {0:F1}% ", hairColor.Confidence * 100));
                }
            }

            // Return the built string.
            return sb.ToString();
        }

        //// Uploads the image file and calls Detect Faces.

        public async Task<Face[]> UploadAndDetectFaces(Bitmap image)
        {
            // The list of Face attributes to return.
            IEnumerable<FaceAttributeType> faceAttributes =
                new FaceAttributeType[] { FaceAttributeType.Gender, FaceAttributeType.Age, FaceAttributeType.Smile, FaceAttributeType.Emotion, FaceAttributeType.Glasses, FaceAttributeType.Hair , FaceAttributeType.FacialHair};

            // Call the Face API.
            try
            {
                using (Stream imageFileStream = new MemoryStream())
                {
                    image.Save(imageFileStream, System.Drawing.Imaging.ImageFormat.Bmp);
                    imageFileStream.Seek(0, SeekOrigin.Begin);
                    Face[] faces = await faceServiceClient.DetectAsync(imageFileStream, returnFaceId: true, returnFaceLandmarks: false, returnFaceAttributes: faceAttributes);
                    return faces;
                }
            }
            // Catch and display Face API errors.
            catch (FaceAPIException f)
            {
                MessageBox.Show(f.ErrorMessage, f.ErrorCode);
                return new Face[0];
            }
            // Catch and display all other errors.
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error");
                return new Face[0];
            }
        }

        public async Task<String> FindAgentName(Guid[] FaceId)
        {
            IdentifyResult[] res = await faceServiceClient.IdentifyAsync(persongroupid, FaceId);
            Guid bestGuess = new Guid();
            foreach (var result in res)
            {
                double maxConfidence = 0;
                
                foreach(var candidate in result.Candidates)
                {
                    if (candidate.Confidence > maxConfidence)
                    {
                        maxConfidence = candidate.Confidence;
                        bestGuess = candidate.PersonId;
                    }
                }
            }
            if (!bestGuess.Equals(new Guid()))
            {
                Person person = await faceServiceClient.GetPersonInPersonGroupAsync(persongroupid, bestGuess);
                return person.Name;
            }
            return "007";
        }

        public async Task<bool> IsGroupTrained()
        {
            try
            {
                TrainingStatus ts = await faceServiceClient.GetPersonGroupTrainingStatusAsync(persongroupid);
                return (ts.Status == Status.Succeeded);
            }
            catch (FaceAPIException)
            {
                return false;
            }
        }
    }
}
