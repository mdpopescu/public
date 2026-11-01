using Dapper;
using Microsoft.Extensions.Options;
using YouNews.Core.Ports;

namespace YouNews.Infrastructure.Storage;

/// <summary>Remembers the user's chosen language model; until one is chosen, the configured model applies.</summary>
public sealed class SqliteModelSelection : ICurrentModel, IModelSelector
{
    public SqliteModelSelection(ISqliteConnectionFactory factory, IOptions<YouNewsOptions> options)
    {
        this.factory = factory;
        configuredModel = options.Value.LanguageModel.Model;
    }

    public async Task<string> GetAsync(CancellationToken cancellationToken)
    {
        var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var connectionScope = connection.ConfigureAwait(false);
        var model = await connection.QuerySingleOrDefaultAsync<string>(new CommandDefinition(SELECT, cancellationToken: cancellationToken)).ConfigureAwait(false);
        return model ?? configuredModel;
    }

    public async Task SelectAsync(string model, CancellationToken cancellationToken)
    {
        var connection = await factory.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var connectionScope = connection.ConfigureAwait(false);
        await connection.ExecuteAsync(new CommandDefinition(UPSERT, new { Model = model }, cancellationToken: cancellationToken)).ConfigureAwait(false);
    }

    //

    private const string SELECT = "SELECT model FROM model_selection WHERE id = 1";

    private const string UPSERT = """
        INSERT INTO model_selection (id, model) VALUES (1, @Model)
        ON CONFLICT(id) DO UPDATE SET model = excluded.model
        """;

    private readonly ISqliteConnectionFactory factory;
    private readonly string configuredModel;
}