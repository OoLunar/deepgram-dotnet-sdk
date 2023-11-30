﻿namespace Deepgram.Tests.Fakes;
public class MockHttpMessageHandler<T> : HttpMessageHandler
{
    private readonly T _response;
    private readonly HttpStatusCode _statusCode;
    public MockHttpMessageHandler(T response, HttpStatusCode statusCode)
    {
        _response = response;
        _statusCode = statusCode;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        => Task.FromResult(new HttpResponseMessage()
        {
            StatusCode = _statusCode,
            Content = new StringContent(JsonSerializer.Serialize(_response))
        });

}
