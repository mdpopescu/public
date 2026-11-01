using YouNews.Infrastructure.Server;

namespace YouNews.Infrastructure.Tests.Server;

[TestClass]
public sealed class HardwareFormatterTests
{
    [TestMethod]
    public void FormatsTwoGpusCpuAndMemory()
    {
        var sut = new HardwareFormatter();
        var gpus = sut.ParseGpuQuery("NVIDIA GeForce RTX 5060 Ti, 16376 MiB\nNVIDIA GeForce RTX 2060 SUPER, 8192 MiB\n");

        var result = sut.Format(gpus, "model name\t: AMD Ryzen 9 3900X 12-Core Processor\n", FREE_OUTPUT);

        Assert.AreEqual(
            "GPUs: NVIDIA GeForce RTX 5060 Ti, 16376 MiB; NVIDIA GeForce RTX 2060 SUPER, 8192 MiB. CPU: AMD Ryzen 9 3900X 12-Core Processor. RAM: 62Gi total.",
            result);
    }

    [TestMethod]
    public void ReadsModelLinesFromDriverInformation()
    {
        var sut = new HardwareFormatter();
        const string information = "Model: \t\t NVIDIA GeForce RTX 2060 SUPER\nIRQ:   \t\t 135\nGPU UUID: \t GPU-1234\nVideo BIOS: \t 90.06.31.00.50\n"
            + "Model: \t\t NVIDIA GeForce RTX 5060 Ti\nIRQ:   \t\t 136\n";

        var gpus = sut.ParseGpuInformation(information);

        CollectionAssert.AreEqual(new[] { "NVIDIA GeForce RTX 2060 SUPER", "NVIDIA GeForce RTX 5060 Ti" }, gpus.ToList());
    }

    [TestMethod]
    public void ReportsUnknownGpusWithoutFailing()
    {
        var sut = new HardwareFormatter();

        var result = sut.Format(null, "model name\t: AMD Ryzen 9 3900X 12-Core Processor", FREE_OUTPUT);

        StringAssert.StartsWith(result, "GPUs: unknown. CPU: AMD Ryzen 9 3900X 12-Core Processor.");
    }

    //

    private const string FREE_OUTPUT =
        "               total        used        free      shared  buff/cache   available\n"
        + "Mem:            62Gi       8.5Gi        15Gi       319Mi        42Gi        54Gi\n"
        + "Swap:           19Gi          0B        19Gi\n";
}