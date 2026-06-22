namespace QuotationAccelerator.Infrastructure.Persistence;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

internal static class InboxSchemaPatcher
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

        await CreateTableIfMissingAsync(
            sqliteConnection,
            """
            CREATE TABLE IF NOT EXISTS mail_account_settings (
                Id INTEGER PRIMARY KEY,
                TenantId TEXT,
                ClientId TEXT,
                MailboxAddress TEXT,
                FolderName TEXT NOT NULL DEFAULT 'Inbox',
                IsConnected INTEGER NOT NULL DEFAULT 0,
                LastFetchedAt TEXT
            );
            """,
            cancellationToken);

        await CreateTableIfMissingAsync(
            sqliteConnection,
            """
            CREATE TABLE IF NOT EXISTS inbox_messages (
                Id TEXT PRIMARY KEY,
                GraphMessageId TEXT NOT NULL UNIQUE,
                Subject TEXT NOT NULL,
                FromAddress TEXT NOT NULL,
                FromDisplayName TEXT,
                ReceivedAt TEXT NOT NULL,
                BodyPreview TEXT,
                BodyText TEXT,
                Category INTEGER NOT NULL DEFAULT 0,
                Status INTEGER NOT NULL DEFAULT 0,
                SuggestedReplyBody TEXT
            );
            """,
            cancellationToken);

        await CreateTableIfMissingAsync(
            sqliteConnection,
            """
            CREATE TABLE IF NOT EXISTS inbox_attachments (
                Id TEXT PRIMARY KEY,
                InboxMessageId TEXT NOT NULL,
                FileName TEXT NOT NULL,
                ContentType TEXT,
                LocalPath TEXT,
                FOREIGN KEY (InboxMessageId) REFERENCES inbox_messages(Id) ON DELETE CASCADE
            );
            """,
            cancellationToken);

        await CreateTableIfMissingAsync(
            sqliteConnection,
            """
            CREATE TABLE IF NOT EXISTS support_tickets (
                Id TEXT PRIMARY KEY,
                InboxMessageId TEXT NOT NULL,
                Subject TEXT NOT NULL,
                FromAddress TEXT NOT NULL,
                Status INTEGER NOT NULL DEFAULT 0,
                Notes TEXT,
                CreatedAt TEXT NOT NULL,
                UpdatedAt TEXT NOT NULL
            );
            """,
            cancellationToken);

        await CreateTableIfMissingAsync(
            sqliteConnection,
            """
            CREATE TABLE IF NOT EXISTS faq_templates (
                Id TEXT PRIMARY KEY,
                KeywordsJson TEXT NOT NULL,
                ReplyBody TEXT NOT NULL
            );
            """,
            cancellationToken);

        await EnsureMailSettingsRowAsync(sqliteConnection, cancellationToken);
        await EnsureDefaultMailResponseSamplesAsync(dbContext, cancellationToken);
    }

    private static async Task CreateTableIfMissingAsync(
        SqliteConnection connection,
        string createSql,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.CommandText = createSql;
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task EnsureMailSettingsRowAsync(
        SqliteConnection connection,
        CancellationToken cancellationToken)
    {
        await using var countCommand = connection.CreateCommand();
        countCommand.CommandText = "SELECT COUNT(*) FROM mail_account_settings;";
        var count = Convert.ToInt32(await countCommand.ExecuteScalarAsync(cancellationToken));
        if (count > 0)
        {
            return;
        }

        await using var insert = connection.CreateCommand();
        insert.CommandText =
            "INSERT INTO mail_account_settings (Id, FolderName, IsConnected) VALUES (1, 'Inbox', 0);";
        await insert.ExecuteNonQueryAsync(cancellationToken);
    }

    private static async Task EnsureDefaultMailResponseSamplesAsync(
        AppDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var existingIds = await dbContext.FaqTemplates
            .AsNoTracking()
            .Select(template => template.Id)
            .ToListAsync(cancellationToken);

        var added = false;
        foreach (var sample in DefaultMailResponseSamples.All)
        {
            if (existingIds.Contains(sample.Id))
            {
                continue;
            }

            dbContext.FaqTemplates.Add(new FaqTemplateEntity
            {
                Id = sample.Id,
                KeywordsJson = sample.KeywordsJson,
                ReplyBody = sample.ReplyBody,
            });
            added = true;
        }

        if (added)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
