using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
// AWS packages
using Amazon.Textract;
using Amazon.S3;
using Amazon.S3.Transfer;

namespace Epic.AI.AWS.OCR
{
    public class AWSTextractOCR
    {
        private const string BUCKET = "hs-extract";
        public static void UploadFileToS3(string fileName, Amazon.Runtime.BasicAWSCredentials _awsCredentials)
        {
            using (var s3Client = new AmazonS3Client(_awsCredentials, Amazon.RegionEndpoint.USWest1))
            {
                var fileTransferUtility = new TransferUtility(s3Client);
                using (Stream sourceStream = File.OpenRead(fileName))
                {
                    fileTransferUtility.Upload(sourceStream, BUCKET, "bc1_test1.png");
                    Console.WriteLine(sourceStream);
                }
            }
        }
        public static async Task<string> AnalyzeDocument(string bucket, string file, Amazon.Runtime.BasicAWSCredentials _awsCredentials)
        {
            var featureTypes = new List<string>();
            featureTypes.Add("FORMS");
            string detectedText = "";
            using (var client = new Amazon.Textract.AmazonTextractClient(_awsCredentials))
            {
                var request = new Amazon.Textract.Model.AnalyzeDocumentRequest
                {
                    Document = new Amazon.Textract.Model.Document()
                    {
                        S3Object = new Amazon.Textract.Model.S3Object()
                        {
                            Bucket = BUCKET,
                            Name = "bc1_test1.png"
                        }
                    },
                    FeatureTypes = featureTypes
                };
                var results = await client.AnalyzeDocumentAsync(request);
                var lineBlocks = (from x in results.Blocks
                                  where x.BlockType == BlockType.LINE
                                  select x).ToList();
                var wordBlocks = (from x in results.Blocks
                                  where x.BlockType == BlockType.WORD
                                  select x).ToList();
                foreach (var block in lineBlocks)
                {
                    detectedText = detectedText + "\n" + block.Text;
                    Console.WriteLine(block.Text);
                }
            }
            return detectedText;
        }
    }
}
