using System;
using System.IO;
using System.Linq;
using FaceRecognitionDotNet;
using Microsoft.Extensions.Configuration;

namespace MissingPeople.Services
{
    public class FaceRecognitionService
    {
        public string DataDirectory { get; } = "D://faces";
        private FaceRecognition _FaceRecognition;

        public FaceRecognitionService(IConfiguration configuration)
        {
            if (!Directory.Exists(DataDirectory))
                Directory.CreateDirectory(DataDirectory);

            _FaceRecognition = FaceRecognition.Create(DataDirectory);
        }

        public double CalculateDistance(string img1, string img2)
        {
            using (var searchImage = FaceRecognition.LoadImageFile(img1))
            {
                var searchEncodings = _FaceRecognition.FaceEncodings(searchImage);
                var targetImage = FaceRecognition.LoadImageFile(img2);
                var targetEncoding = _FaceRecognition.FaceEncodings(targetImage);
                
                var distance = FaceRecognition.FaceDistance(searchEncodings.First(), targetEncoding.First());
                Console.WriteLine($"Distance: {distance}");  
                foreach (var encoding in targetEncoding) encoding.Dispose();
                foreach (var encoding in searchEncodings) encoding.Dispose();
                return distance;
            }
        }
    }
}
