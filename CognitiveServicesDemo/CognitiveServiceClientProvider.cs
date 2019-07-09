using Microsoft.Rest;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CognitiveServicesDemo
{    /// <summary>
     /// Allows authentication to the API using a basic apiKey mechanism
     /// </summary>
    public class ApiKeyServiceClientCredentials : ServiceClientCredentials
    {
        private readonly string subscriptionKey;

        /// <summary>
        /// Creates a new instance of the ApiKeyServiceClientCredentails class
        /// </summary>
        /// <param name="subscriptionKey">The subscription key to authenticate and authorize as</param>
        public ApiKeyServiceClientCredentials(string subscriptionKey)
        {
            this.subscriptionKey = subscriptionKey;
        }

        /// <summary>
        /// Add the Basic Authentication Header to each outgoing request
        /// </summary>
        /// <param name="request">The outgoing request</param>
        /// <param name="cancellationToken">A token to cancel the operation</param>
        public override Task ProcessHttpRequestAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            request.Headers.Add("Ocp-Apim-Subscription-Key", this.subscriptionKey);

            return Task.CompletedTask;
        }
    }

    public class CognitiveServiceClientProvider
    {
        private string _subscriptionKey;
        private string _endpointUrl;
        private DelegatingHandler[] _handlers;
        public CognitiveServiceClientProvider(string subscriptionKey, string endpointUrl, DelegatingHandler[] handlers = null)
        {
            _subscriptionKey = subscriptionKey;
            _endpointUrl = endpointUrl;
            _handlers = handlers;
        }

        public T GetClient<T>(Func<ServiceClientCredentials, DelegatingHandler[], T> client_init)
        {
            var client = client_init(new ApiKeyServiceClientCredentials(_subscriptionKey), _handlers);
            client.GetType().GetProperty("Endpoint").SetValue(client, _endpointUrl, null);
            return client;
        }
    }
}