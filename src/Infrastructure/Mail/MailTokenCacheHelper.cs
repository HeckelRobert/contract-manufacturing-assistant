namespace QuotationAccelerator.Infrastructure.Mail;

using System.Text.Json;
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;

internal static class MailTokenCacheHelper
{
    private const string CacheFileName = "msal_mail_token_cache.bin";
    private static readonly object LockObject = new();
    private static MsalCacheHelper? _cacheHelper;

    public static void Register(IPublicClientApplication app, string appDirectory)
    {
        lock (LockObject)
        {
            if (_cacheHelper is not null)
            {
                return;
            }

            var storageProperties = new StorageCreationPropertiesBuilder(
                    CacheFileName,
                    appDirectory)
                .Build();

            _cacheHelper = MsalCacheHelper.CreateAsync(storageProperties).GetAwaiter().GetResult();
            _cacheHelper.RegisterCache(app.UserTokenCache);
        }
    }
}

internal static class FaqTemplateJson
{
    public static string SerializeKeywords(IReadOnlyList<string> keywords) =>
        JsonSerializer.Serialize(keywords);

    public static IReadOnlyList<string> DeserializeKeywords(string json) =>
        JsonSerializer.Deserialize<List<string>>(json) ?? [];
}
