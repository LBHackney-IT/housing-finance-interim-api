using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Http;
using IHttpClientFactory = Google.Apis.Http.IHttpClientFactory;

namespace HousingFinanceInterimApi.Tests.V1.Factories
{
    public class FakeHttpClientFactory : IHttpClientFactory
    {
        private readonly ConfigurableMessageHandler _handler;

        [SuppressMessage("ReSharper", "CA2000")]
        public FakeHttpClientFactory(Func<HttpRequestMessage, HttpResponseMessage> handler)
            : this(new ConfigurableMessageHandler(new MockableMessageHandler(handler)))
        {
        }

        private FakeHttpClientFactory(ConfigurableMessageHandler handler) => _handler = handler;

        public ConfigurableHttpClient CreateHttpClient(CreateHttpClientArgs args) =>
            new ConfigurableHttpClient(_handler, true);

        private class MockableMessageHandler : HttpMessageHandler
        {
            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handler;

            public MockableMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler) =>
                _handler = (req, token) => Task.FromResult(handler(req));

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
                CancellationToken cancellationToken) => _handler(request, cancellationToken);
        }
    }
}
