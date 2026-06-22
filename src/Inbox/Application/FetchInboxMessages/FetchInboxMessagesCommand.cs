namespace QuotationAccelerator.Inbox.Application.FetchInboxMessages;

using Microsoft.Extensions.Logging;
using QuotationAccelerator.Inbox.Application.Abstractions;
using QuotationAccelerator.Inbox.Domain;
using QuotationAccelerator.SharedKernel.Abstractions;
using QuotationAccelerator.SharedKernel.Results;

public sealed record FetchInboxMessagesCommand : ICommand<Result<int>>;

public sealed class FetchInboxMessagesHandler(
    IMailClient mailClient,
    IMailAccountRepository accountRepository,
    IInboxMessageRepository messageRepository,
    IFaqTemplateRepository faqTemplateRepository,
    IEmailCategorizationService categorizationService,
    IMailAttachmentStore attachmentStore,
    ILogger<FetchInboxMessagesHandler> logger)
    : ICommandHandler<FetchInboxMessagesCommand, Result<int>>
{
    public async Task<Result<int>> HandleAsync(FetchInboxMessagesCommand command, CancellationToken cancellationToken)
    {
        if (!mailClient.IsConnected)
        {
            return Result<int>.Failure("Mail account is not connected.");
        }

        var settings = await accountRepository.GetSettingsAsync(cancellationToken);
        var fetchResult = await mailClient.FetchMessagesAsync(settings.LastFetchedAt, cancellationToken);
        if (fetchResult.IsFailure)
        {
            return Result<int>.Failure(fetchResult.Errors);
        }

        var faqTemplates = await faqTemplateRepository.GetAllAsync(cancellationToken);
        var imported = 0;

        foreach (var remote in fetchResult.Value!)
        {
            if (await messageRepository.GetByGraphMessageIdAsync(remote.GraphMessageId, cancellationToken) is not null)
            {
                continue;
            }

            var messageId = Guid.NewGuid().ToString("N");
            var attachments = new List<InboxAttachment>();

            foreach (var remoteAttachment in remote.Attachments)
            {
                var attachmentId = Guid.NewGuid().ToString("N");
                string? localPath = null;

                if (remoteAttachment.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    var download = await mailClient.DownloadAttachmentAsync(
                        remote.GraphMessageId,
                        remoteAttachment.Id,
                        cancellationToken);

                    if (download.IsSuccess)
                    {
                        localPath = await attachmentStore.SaveAttachmentAsync(
                            messageId,
                            remoteAttachment.FileName,
                            download.Value!,
                            cancellationToken);
                    }
                }

                attachments.Add(new InboxAttachment
                {
                    Id = attachmentId,
                    FileName = remoteAttachment.FileName,
                    ContentType = remoteAttachment.ContentType,
                    LocalPath = localPath,
                });
            }

            var message = new InboxMessage
            {
                Id = messageId,
                GraphMessageId = remote.GraphMessageId,
                Subject = remote.Subject,
                FromAddress = remote.FromAddress,
                FromDisplayName = remote.FromDisplayName,
                ReceivedAt = remote.ReceivedAt,
                BodyPreview = remote.BodyPreview,
                BodyText = remote.BodyText,
                Attachments = attachments,
            };

            var categorization = categorizationService.Categorize(message, faqTemplates);
            var categorizedMessage = new InboxMessage
            {
                Id = message.Id,
                GraphMessageId = message.GraphMessageId,
                Subject = message.Subject,
                FromAddress = message.FromAddress,
                FromDisplayName = message.FromDisplayName,
                ReceivedAt = message.ReceivedAt,
                BodyPreview = message.BodyPreview,
                BodyText = message.BodyText,
                Attachments = message.Attachments,
                Category = categorization.Category,
                SuggestedReplyBody = categorization.SuggestedReplyBody,
            };

            await messageRepository.UpsertAsync(categorizedMessage, cancellationToken);
            imported++;
        }

        await accountRepository.SetLastFetchedAtAsync(DateTimeOffset.UtcNow, cancellationToken);
        logger.LogInformation("Imported {ImportedCount} inbox messages", imported);
        return Result<int>.Success(imported);
    }
}
