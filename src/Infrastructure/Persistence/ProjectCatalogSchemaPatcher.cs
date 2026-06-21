namespace QuotationAccelerator.Infrastructure.Persistence;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

internal static class ProjectCatalogSchemaPatcher
{
    public static async Task ApplyAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        var connection = dbContext.Database.GetDbConnection();
        if (connection is not SqliteConnection sqliteConnection)
        {
            return;
        }

        if (sqliteConnection.State != System.Data.ConnectionState.Open)
        {
            await sqliteConnection.OpenAsync(cancellationToken);
        }

        await AddColumnIfMissingAsync(sqliteConnection, "projects", "PartDescription", "TEXT", cancellationToken);
        await AddColumnIfMissingAsync(sqliteConnection, "projects", "DrawingNumber", "TEXT", cancellationToken);
        await AddColumnIfMissingAsync(sqliteConnection, "projects", "Dimensions", "TEXT", cancellationToken);
    }

    private static async Task AddColumnIfMissingAsync(
        SqliteConnection connection,
        string tableName,
        string columnName,
        string columnType,
        CancellationToken cancellationToken)
    {
        var exists = false;
        await using (var command = connection.CreateCommand())
        {
            command.CommandText = $"PRAGMA table_info({tableName});";
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var name = reader.GetString(1);
                if (string.Equals(name, columnName, StringComparison.OrdinalIgnoreCase))
                {
                    exists = true;
                    break;
                }
            }
        }

        if (exists)
        {
            return;
        }

        await using var alter = connection.CreateCommand();
        alter.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {columnType};";
        await alter.ExecuteNonQueryAsync(cancellationToken);
    }
}
