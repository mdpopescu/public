using System.Text.Json;
using Microsoft.Extensions.Options;
using YouNews.Core.Ports;

namespace YouNews.Infrastructure.Server;

public sealed class ServerInspector : IServerInspector
{
    public ServerInspector(HttpClient httpClient, IRemoteCommandRunner runner, IOptions<YouNewsOptions> options)
    {
        this.httpClient = httpClient;
        this.runner = runner;
        this.options = options.Value;
    }

    public async Task<IReadOnlyList<string>> ListModelsAsync(CancellationToken cancellationToken)
    {
        var url = $"{options.LanguageModel.BaseUrl.TrimEnd('/')}/models";
        using var response = await httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        using var document = JsonDocument.Parse(json);
        return document.RootElement.GetProperty("data").EnumerateArray()
            .Select(model => model.GetProperty("id").GetString()!)
            .Order(StringComparer.Ordinal)
            .ToList();
    }

    public async Task<string> DescribeHardwareAsync(CancellationToken cancellationToken)
    {
        var gpus = await ListGpusAsync(cancellationToken).ConfigureAwait(false);
        var cpu = await runner.RunAsync(CPU_COMMAND, cancellationToken).ConfigureAwait(false);
        var memory = await runner.RunAsync(MEMORY_COMMAND, cancellationToken).ConfigureAwait(false);
        return formatter.Format(gpus, cpu, memory);
    }

    //

    private const string GPU_QUERY_COMMAND = "nvidia-smi --query-gpu=name,memory.total --format=csv,noheader";
    private const string GPU_INFORMATION_COMMAND = "cat /proc/driver/nvidia/gpus/*/information";
    private const string CPU_COMMAND = "grep -m1 \"model name\" /proc/cpuinfo";
    private const string MEMORY_COMMAND = "free -h";

    private readonly HttpClient httpClient;
    private readonly IRemoteCommandRunner runner;
    private readonly YouNewsOptions options;
    private readonly HardwareFormatter formatter = new();

    private async Task<IReadOnlyList<string>?> ListGpusAsync(CancellationToken cancellationToken)
    {
        try
        {
            return formatter.ParseGpuQuery(await runner.RunAsync(GPU_QUERY_COMMAND, cancellationToken).ConfigureAwait(false));
        }
        catch (Exception) when (!cancellationToken.IsCancellationRequested)
        {
            return await ListGpusFromDriverAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private async Task<IReadOnlyList<string>?> ListGpusFromDriverAsync(CancellationToken cancellationToken)
    {
        try
        {
            return formatter.ParseGpuInformation(await runner.RunAsync(GPU_INFORMATION_COMMAND, cancellationToken).ConfigureAwait(false));
        }
        catch (Exception) when (!cancellationToken.IsCancellationRequested)
        {
            return null;
        }
    }
}