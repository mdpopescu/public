using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using YouNews.Infrastructure.LanguageModels;

namespace YouNews.Infrastructure.Tests.LanguageModels;

[TestClass]
public sealed class OpenAiCompatibleLanguageModelTests : IDisposable
{
    [TestInitialize]
    public void Initialize()
    {
        handler = new StubHandler();
        timeProvider = new InstantTimeProvider();
        var options = new YouNewsOptions { LanguageModel = { BaseUrl = "http://llm.test/v1/", Model = "gemma4-26b", MaxTokens = 16000 } };
        sut = new OpenAiCompatibleLanguageModel(new HttpClient(handler), Options.Create(options), timeProvider);
    }

    [TestMethod]
    public async Task PostsModelTokensAndMessagesToChatCompletions()
    {
        handler.Enqueue(Answer("hello"));

        await sut.CompleteAsync("the system", "the user", true, CancellationToken.None);

        Assert.AreEqual("http://llm.test/v1/chat/completions", handler.Urls.Single());
        var body = handler.Bodies.Single().RootElement;
        Assert.AreEqual("gemma4-26b", body.GetProperty("model").GetString());
        Assert.AreEqual(16000, body.GetProperty("max_tokens").GetInt32());
        var messages = body.GetProperty("messages");
        Assert.AreEqual("system", messages[0].GetProperty("role").GetString());
        Assert.AreEqual("the system", messages[0].GetProperty("content").GetString());
        Assert.AreEqual("user", messages[1].GetProperty("role").GetString());
        Assert.AreEqual("the user", messages[1].GetProperty("content").GetString());
    }

    [TestMethod]
    public async Task ThinkingOffDisablesItThroughTheChatTemplate()
    {
        handler.Enqueue(Answer("hello"));

        await sut.CompleteAsync("s", "u", false, CancellationToken.None);

        var enabled = handler.Bodies.Single().RootElement.GetProperty("chat_template_kwargs").GetProperty("enable_thinking");
        Assert.AreEqual(JsonValueKind.False, enabled.ValueKind);
    }

    [TestMethod]
    public async Task ThinkingOnSendsNoTemplateArguments()
    {
        handler.Enqueue(Answer("hello"));

        await sut.CompleteAsync("s", "u", true, CancellationToken.None);

        Assert.IsFalse(handler.Bodies.Single().RootElement.TryGetProperty("chat_template_kwargs", out _), "The server default must apply when thinking is on.");
    }

    [TestMethod]
    public async Task ReturnsContentAndIgnoresReasoning()
    {
        handler.Enqueue(Json("""{"choices":[{"message":{"content":"the answer","reasoning_content":"secret musings"},"finish_reason":"stop"}]}"""));

        var result = await sut.CompleteAsync("s", "u", true, CancellationToken.None);

        Assert.AreEqual("the answer", result);
    }

    [TestMethod]
    public async Task EmptyContentAtLengthSaysTheBudgetRanOut()
    {
        handler.Enqueue(Json("""{"choices":[{"message":{"content":"","reasoning_content":"long thoughts"},"finish_reason":"length"}]}"""));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CompleteAsync("s", "u", true, CancellationToken.None));

        StringAssert.Contains(exception.Message, "finish_reason length");
        StringAssert.Contains(exception.Message, "budget ran out");
        Assert.HasCount(1, handler.Urls, "An exhausted budget would just run out again, so it must not be retried.");
    }

    [TestMethod]
    public async Task AnswerCutOffAtLengthIsAnErrorRatherThanAShortAnswer()
    {
        handler.Enqueue(Json("""{"choices":[{"message":{"content":"The first paragraph and then"},"finish_reason":"length"}]}"""));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CompleteAsync("s", "u", true, CancellationToken.None));

        StringAssert.Contains(exception.Message, "cut off");
    }

    [TestMethod]
    public async Task EmptyContentForAnotherReasonNamesTheReason()
    {
        handler.Enqueue(Json("""{"choices":[{"message":{"content":null},"finish_reason":"content_filter"}]}"""));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CompleteAsync("s", "u", true, CancellationToken.None));

        StringAssert.Contains(exception.Message, "finish_reason content_filter");
        Assert.DoesNotContain("budget", exception.Message, exception.Message);
    }

    [TestMethod]
    public async Task ResponseWithoutChoicesFailsClearly()
    {
        handler.Enqueue(Json("""{"error":"nothing"}"""));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CompleteAsync("s", "u", true, CancellationToken.None));

        StringAssert.Contains(exception.Message, "no choices");
    }

    [TestMethod]
    public async Task NonJsonResponseFailsClearly()
    {
        handler.Enqueue(Json("<html>proxy page</html>"));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => sut.CompleteAsync("s", "u", true, CancellationToken.None));

        StringAssert.Contains(exception.Message, "not JSON");
        StringAssert.Contains(exception.Message, "proxy page");
    }

    [TestMethod]
    public async Task ClientErrorIsNotRetriedAndReportsStatusAndShortBody()
    {
        handler.Enqueue(Status(HttpStatusCode.BadRequest, "bad model " + new string('x', 5000)));

        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => sut.CompleteAsync("s", "u", true, CancellationToken.None));

        StringAssert.Contains(exception.Message, "400");
        StringAssert.Contains(exception.Message, "bad model");
        Assert.IsLessThan(600, exception.Message.Length, "The body in the message must be shortened: " + exception.Message.Length);
        Assert.HasCount(1, handler.Urls);
        Assert.IsEmpty(timeProvider.Delays);
    }

    [TestMethod]
    public async Task ServerErrorIsRetriedOnceAfterTheDelay()
    {
        handler.Enqueue(Status(HttpStatusCode.BadGateway, "swapping"));
        handler.Enqueue(Answer("after the swap"));

        var result = await sut.CompleteAsync("s", "u", false, CancellationToken.None);

        Assert.AreEqual("after the swap", result);
        Assert.HasCount(2, handler.Urls);
        Assert.HasCount(1, timeProvider.Delays);
        Assert.IsGreaterThan(TimeSpan.Zero, timeProvider.Delays[0]);
        Assert.AreEqual(handler.RawBodies[0], handler.RawBodies[1], "The retry must send the same request.");
    }

    [TestMethod]
    public async Task SecondServerErrorIsThrown()
    {
        handler.Enqueue(Status(HttpStatusCode.ServiceUnavailable, "first"));
        handler.Enqueue(Status(HttpStatusCode.ServiceUnavailable, "still down"));
        handler.Enqueue(Answer("must not be reached"));

        var exception = await Assert.ThrowsAsync<HttpRequestException>(() => sut.CompleteAsync("s", "u", true, CancellationToken.None));

        StringAssert.Contains(exception.Message, "503");
        StringAssert.Contains(exception.Message, "still down");
        Assert.HasCount(2, handler.Urls);
    }

    [TestMethod]
    public async Task ConnectionFailureIsRetriedOnce()
    {
        handler.Enqueue(_ => throw new HttpRequestException("connection refused"));
        handler.Enqueue(Answer("back"));

        var result = await sut.CompleteAsync("s", "u", true, CancellationToken.None);

        Assert.AreEqual("back", result);
        Assert.HasCount(1, timeProvider.Delays);
    }

    [TestMethod]
    public async Task CancellationDuringRequestIsNotRetried()
    {
        handler.Enqueue(_ =>
        {
            cancellation.Cancel();
            throw new TaskCanceledException();
        });
        handler.Enqueue(Answer("must not be reached"));

        await Assert.ThrowsAsync<OperationCanceledException>(() => sut.CompleteAsync("s", "u", true, cancellation.Token));

        Assert.HasCount(1, handler.Urls);
        Assert.IsEmpty(timeProvider.Delays);
    }

    [TestMethod]
    public async Task CancellationBeforeTheRetryStopsIt()
    {
        handler.Enqueue(_ =>
        {
            cancellation.Cancel();
            throw new HttpRequestException("connection reset");
        });
        handler.Enqueue(Answer("must not be reached"));

        await Assert.ThrowsAsync<OperationCanceledException>(() => sut.CompleteAsync("s", "u", true, cancellation.Token));

        Assert.HasCount(1, handler.Urls);
    }

    [TestMethod]
    public async Task HttpClientTimeoutIsReportedAndNotRetried()
    {
        handler.Enqueue(_ => throw new TaskCanceledException("timed out", new TimeoutException()));
        handler.Enqueue(Answer("must not be reached"));

        var exception = await Assert.ThrowsAsync<TimeoutException>(() => sut.CompleteAsync("s", "u", true, CancellationToken.None));

        StringAssert.Contains(exception.Message, "30 minutes");
        Assert.HasCount(1, handler.Urls);
    }

    [TestMethod]
    public void TimeoutComesFromTheOptions()
    {
        var options = new YouNewsOptions { LanguageModel = { TimeoutMinutes = 7 } };
        var httpClient = new HttpClient(new StubHandler());

        _ = new OpenAiCompatibleLanguageModel(httpClient, Options.Create(options), TimeProvider.System);

        Assert.AreEqual(TimeSpan.FromMinutes(7), httpClient.Timeout);
    }

    public void Dispose() => cancellation.Dispose();

    //

    private readonly CancellationTokenSource cancellation = new();

    private StubHandler handler = null!;
    private InstantTimeProvider timeProvider = null!;
    private OpenAiCompatibleLanguageModel sut = null!;

    private static Func<HttpRequestMessage, HttpResponseMessage> Answer(string content) =>
        Json(JsonSerializer.Serialize(new { choices = new[] { new { message = new { content }, finish_reason = "stop" } } }));

    private static Func<HttpRequestMessage, HttpResponseMessage> Json(string body) =>
        _ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    private static Func<HttpRequestMessage, HttpResponseMessage> Status(HttpStatusCode status, string body) =>
        _ => new HttpResponseMessage(status) { Content = new StringContent(body) };

    private sealed class StubHandler : HttpMessageHandler
    {
        public List<string> Urls { get; } = [];

        public List<string> RawBodies { get; } = [];

        public IEnumerable<JsonDocument> Bodies => RawBodies.Select(body => JsonDocument.Parse(body));

        public void Enqueue(Func<HttpRequestMessage, HttpResponseMessage> response) => responses.Enqueue(response);

        //

        private readonly Queue<Func<HttpRequestMessage, HttpResponseMessage>> responses = new();

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Urls.Add(request.RequestUri!.ToString());
            RawBodies.Add(await request.Content!.ReadAsStringAsync(cancellationToken));
            return responses.Dequeue()(request);
        }
    }

    private sealed class InstantTimeProvider : TimeProvider
    {
        public List<TimeSpan> Delays { get; } = [];

        public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
        {
            Delays.Add(dueTime);
            callback(state);
            return new FiredTimer();
        }

        //

        private sealed class FiredTimer : ITimer
        {
            public bool Change(TimeSpan dueTime, TimeSpan period) => true;

            public void Dispose()
            {
            }

            public ValueTask DisposeAsync() => ValueTask.CompletedTask;
        }
    }
}