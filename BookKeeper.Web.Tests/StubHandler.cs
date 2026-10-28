using System.Net;

namespace BookKeeper.Web.Tests;

// Answers every request with the given status code and body, so BooksClient can be tested without the API.
public class StubHandler(HttpStatusCode status, string body = "") : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromResult(new HttpResponseMessage(status) { Content = new StringContent(body) });
}
