using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServicesDemo
{
    class Program
    {
        /**set Subscription Key and End Point Url here*/
        private static readonly string subscriptionKey = "xxxxxxxxxxxxxxxxxxx";
        private static readonly string endpointUrl = "https://xxxxxxxxxxx.api.cognitive.microsoft.com/";
        private static ComputerVisionClient client = new CognitiveServiceClientProvider(subscriptionKey, endpointUrl)
                .GetClient((c, h) => new ComputerVisionClient(c, handlers: h));
        private static string DefaultImageUrl = "https://cognitiveservicesdemo.blob.core.windows.net/images/kids.jpg";

        static async Task Main(string[] args)
        {
            string host = "https://api.cognitive.microsofttranslator.com";
            string route = "/translate?api-version=3.0&to=de&to=sv";
            string subscriptionKey = "xxxxxxxxxxxxxxxxxxx";

            // Prompts you for text to translate. If you'd prefer, you can
            // provide a string as textToTranslate.
            Console.Write("Type the phrase you'd like to translate? ");
            string textToTranslate = Console.ReadLine();
            await TranslateTextRequest(subscriptionKey, host, route, textToTranslate);

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
            Console.ReadLine();
        }

        // Async call to the Translator Text API
        static public async Task TranslateTextRequest(string subscriptionKey, string host, string route, string inputText)
        {
            object[] body = new object[] { new { Text = inputText } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                // Build the request.
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri(host + route);
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
                request.Headers.Add("Ocp-Apim-Subscription-Region", "australiaeast");

                // Send the request and get response.
                HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
                // Read response as a string.
                string result = await response.Content.ReadAsStringAsync();
                TranslationResult[] deserializedOutput = JsonConvert.DeserializeObject<TranslationResult[]>(result);
                // Iterate over the deserialized results.
                foreach (TranslationResult o in deserializedOutput)
                {
                    // Print the detected input languge and confidence score.
                    Console.WriteLine("Detected input language: {0}\nConfidence score: {1}\n", o.DetectedLanguage.Language, o.DetectedLanguage.Score);
                    // Iterate over the results and print each translation.
                    foreach (Translation t in o.Translations)
                    {
                        Console.WriteLine("Translated to {0}: {1}", t.To, t.Text);
                    }
                }
                Console.ReadLine();
            }
        }
    }

    public class TranslationResult
    {
        public DetectedLanguage DetectedLanguage { get; set; }
        public TextResult SourceText { get; set; }
        public Translation[] Translations { get; set; }
    }

    public class DetectedLanguage
    {
        public string Language { get; set; }
        public float Score { get; set; }
    }

    public class TextResult
    {
        public string Text { get; set; }
        public string Script { get; set; }
    }

    public class Translation
    {
        public string Text { get; set; }
        public TextResult Transliteration { get; set; }
        public string To { get; set; }
        public Alignment Alignment { get; set; }
        public SentenceLength SentLen { get; set; }
    }

    public class Alignment
    {
        public string Proj { get; set; }
    }

    public class SentenceLength
    {
        public int[] SrcSentLen { get; set; }
        public int[] TransSentLen { get; set; }
    }
}
