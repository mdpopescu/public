using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using YouNews.Infrastructure.Server;

namespace YouNews.Infrastructure.Tests.Server;

[TestClass]
public sealed class ServerInspectorTests
{
    [TestInitialize]
    public void Initialize()
    {
        handler = new StubHandler();
        runner = new FakeRunner();
        runner.Outputs[GPU_QUERY] = "NVIDIA GeForce RTX 5060 Ti, 16376 MiB\nNVIDIA GeForce RTX 2060 SUPER, 8192 MiB\n";
        runner.Outputs[CPU_QUERY] = "model name\t: AMD Ryzen 9 3900X 12-Core Processor\n";
        runner.Outputs["free -h"] = FREE_OUTPUT;
        var options = Options.Create(new YouNewsOptions { LanguageModel = { BaseUrl = "http://llm.test:8080/v1/" } });
        sut = new ServerInspector(new HttpClient(handler), runner, options);
    }

    [TestMethod]
    public async Task ListsSortedModelIdsFromModelsEndpoint()
    {
        handler.Body = "{\"object\":\"list\",\"data\":[{\"id\":\"qwen3-8b\",\"object\":\"model\"},{\"id\":\"gemma4-26b\",\"object\":\"model\"}]}";

        var models = await sut.ListModelsAsync(CancellationToken.None);

        CollectionAssert.AreEqual(new[] { "gemma4-26b", "qwen3-8b" }, models.ToList());
        Assert.AreEqual("http://llm.test:8080/v1/models", handler.RequestUrl);
    }

    [TestMethod]
    public async Task ThrowsWhenModelsEndpointFails()
    {
        handler.Status = HttpStatusCode.ServiceUnavailable;

        await Assert.ThrowsExactlyAsync<HttpRequestException>(() => sut.ListModelsAsync(CancellationToken.None));
    }

    [TestMethod]
    public async Task DescribesHardwareFromThreeCommands()
    {
        var result = await sut.DescribeHardwareAsync(CancellationToken.None);

        Assert.AreEqual(
            "GPUs: NVIDIA GeForce RTX 5060 Ti, 16376 MiB; NVIDIA GeForce RTX 2060 SUPER, 8192 MiB. CPU: AMD Ryzen 9 3900X 12-Core Processor. RAM: 62Gi total.",
            result);
    }

    [TestMethod]
    public async Task FallsBackToDriverInformationWhenNvidiaSmiFails()
    {
        runner.Outputs.Remove(GPU_QUERY);
        runner.Outputs[GPU_FALLBACK] = "Model: \t\t NVIDIA GeForce RTX 2060 SUPER\nIRQ:   \t\t 135\n";

        var result = await sut.DescribeHardwareAsync(CancellationToken.None);

        StringAssert.StartsWith(result, "GPUs: NVIDIA GeForce RTX 2060 SUPER. CPU: AMD Ryzen");
    }

    [TestMethod]
    public async Task ReportsGpusUnknownWhenBothGpuCommandsFail()
    {
        runner.Outputs.Remove(GPU_QUERY);

        var result = await sut.DescribeHardwareAsync(CancellationToken.None);

        Assert.AreEqual("GPUs: unknown. CPU: AMD Ryzen 9 3900X 12-Core Processor. RAM: 62Gi total.", result);
    }

    [TestMethod]
    public async Task FailsWhenCpuCommandFails()
    {
        runner.Outputs.Remove(CPU_QUERY);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => sut.DescribeHardwareAsync(CancellationToken.None));
    }

    [TestMethod]
    public async Task PropagatesCancellationInsteadOfFallingBack()
    {
        using var source = new CancellationTokenSource();
        await source.CancelAsync();
        runner.CancelOnRun = true;

        await Assert.ThrowsAsync<OperationCanceledException>(() => sut.DescribeHardwareAsync(source.Token));
        Assert.DoesNotContain(GPU_FALLBACK, runner.Commands);
    }

    //

    private const string GPU_QUERY = "nvidia-smi --query-gpu=name,memory.total --format=csv,noheader";
    private const string GPU_FALLBACK = "cat /proc/driver/nvidia/gpus/*/information";
    private const string CPU_QUERY = "grep -m1 \"model name\" /proc/cpuinfo";

    private const string FREE_OUTPUT =
        "               total        used        free      shared  buff/cache   available\n"
        + "Mem:            62Gi       8.5Gi        15Gi       319Mi        42Gi        54Gi\n"
        + "Swap:           19Gi          0B        19Gi\n";

    private StubHandler handler = null!;
    private FakeRunner runner = null!;
    private ServerInspector sut = null!;

    private sealed class StubHandler : HttpMessageHandler
    {
        public string Body { get; set; } = "{\"data\":[]}";

        public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;

        public string? RequestUrl { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestUrl = request.RequestUri?.ToString();
            return Task.FromResult(new HttpResponseMessage(Status) { Content = new StringContent(Body, Encoding.UTF8, "application/json") });
        }
    }

    private sealed class FakeRunner : IRemoteCommandRunner
    {
        public Dictionary<string, string> Outputs { get; } = new();

        public List<string> Commands { get; } = [];

        public bool CancelOnRun { get; set; }

        public Task<string> RunAsync(string command, CancellationToken cancellationToken)
        {
            Commands.Add(command);
            if (CancelOnRun)
                cancellationToken.ThrowIfCancellationRequested();

            return Outputs.TryGetValue(command, out var output)
                ? Task.FromResult(output)
                : throw new InvalidOperationException($"`{command}` exited with code 127");
        }
    }
}