namespace YouNews.Infrastructure.Server;

/// <summary>Turns the raw output of the hardware commands into readable text.</summary>
public sealed class HardwareFormatter
{
    public string Format(IReadOnlyList<string>? gpus, string cpuInfo, string memoryInfo)
    {
        var gpuText = gpus is { Count: > 0 } ? string.Join("; ", gpus) : "unknown";
        return $"GPUs: {gpuText}. CPU: {ParseCpu(cpuInfo)}. RAM: {ParseMemory(memoryInfo)} total.";
    }

    /// <summary>Reads the "name, memory" lines printed by nvidia-smi in csv format.</summary>
    public IReadOnlyList<string> ParseGpuQuery(string output) => Lines(output);

    /// <summary>Reads the Model lines of the files under /proc/driver/nvidia/gpus.</summary>
    public IReadOnlyList<string> ParseGpuInformation(string output) =>
        Lines(output)
            .Where(line => line.StartsWith("Model:", StringComparison.Ordinal))
            .Select(line => line["Model:".Length..].Trim())
            .ToList();

    //

    private static IReadOnlyList<string> Lines(string output) =>
        output.Split(['\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static string ParseCpu(string cpuInfo)
    {
        var line = Lines(cpuInfo).FirstOrDefault(l => l.StartsWith("model name", StringComparison.Ordinal));
        var separator = line?.IndexOf(':') ?? -1;
        return separator < 0 ? "unknown" : line![(separator + 1)..].Trim();
    }

    private static string ParseMemory(string memoryInfo)
    {
        var line = Lines(memoryInfo).FirstOrDefault(l => l.StartsWith("Mem:", StringComparison.Ordinal));
        var columns = line?.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return columns is { Length: > 1 } ? columns[1] : "unknown";
    }
}