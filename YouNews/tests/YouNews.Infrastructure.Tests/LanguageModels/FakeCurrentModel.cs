using YouNews.Core.Ports;

namespace YouNews.Infrastructure.Tests.LanguageModels;

internal sealed class FakeCurrentModel : ICurrentModel
{
    public string Model { get; set; }

    public FakeCurrentModel(string model)
    {
        Model = model;
    }

    public Task<string> GetAsync(CancellationToken cancellationToken) => Task.FromResult(Model);
}