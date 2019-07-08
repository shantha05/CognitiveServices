using Microsoft.Azure.CognitiveServices.Client.Common;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ComputerVisionApplicationDemo
{
    class Program
    {
        /**set Subscription Key and End Point Url here*/
        private static readonly string subscriptionKey = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
        private static readonly string endpointUrl = "https://xxxxxxxxxxxxxxxx.cognitiveservices.azure.com/";

        private static ComputerVisionClient client = new CognitiveServiceClientProvider(subscriptionKey, endpointUrl)
                .GetClient((c, h) => new ComputerVisionClient(c, handlers: h));

        private static string DefaultImageUrl = "https://cognitiveservicesdemo.blob.core.windows.net/images/kids.jpg";

        static async Task MainAsync()
        {
            try
            {
                Console.WriteLine("Please input image url or locate a local image file. If input is empty, example image will be used.");
                var imagePath = Console.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(imagePath))
                {
                    Console.WriteLine($"No url or file specified, use the example {DefaultImageUrl}");
                    imagePath = DefaultImageUrl;
                }
                System.Diagnostics.Process.Start(imagePath);

                var result = await client.DescribeImageAsync(imagePath);

                Console.WriteLine(Environment.NewLine + $"Analyze result of {imagePath} is");
                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void Main(string[] args)
        {
            Task.Run<Task>(async () =>
            {
                await MainAsync();
            }).Unwrap().GetAwaiter().GetResult();

            Console.WriteLine(Environment.NewLine + "Press ENTER to exit.");
            Console.ReadKey();
        }
    }
}
