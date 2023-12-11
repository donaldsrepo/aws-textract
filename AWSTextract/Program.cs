// System packages
using System.Threading.Tasks;
using System;
using System.Configuration;
using Epic.AI.AWS.OCR;

/*
* need to run:
* dotnet add package AWSSDK.Textract --version 3.7.300.16" from within the console project directory
* dotnet add package AWSSDK.Core
* dotnet add package AWSSDK.S3
* dotnet add package System.Configuration.ConfigurationManager
*/

namespace AWSTextract
{
    internal class Program
    {
        private const string BUCKET = "hs-extract";
        static async Task Main(string[] args)
        {
            var accessKey = ConfigurationManager.AppSettings["accessKey"];
            var secretKey = ConfigurationManager.AppSettings["secretKey"];
            var _awsCredentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
            var image_name = "C:\\Users\\Don\\Documents\\DataScience\\AWSProjects\\aws-textract\\AWSTextract\\bc1.png";
            string outputText = "";
            AWSTextractOCR.UploadFileToS3(image_name, _awsCredentials);
            // this will allow you to run several of these at a time
            // var tasks = new List<Task> { AnalyzeDocument(BUCKET, image_name) };
            // await Task.WhenAll(tasks);
            outputText = await AWSTextractOCR.AnalyzeDocument(BUCKET, image_name, _awsCredentials);
            Console.WriteLine(outputText);
        }
    }
}