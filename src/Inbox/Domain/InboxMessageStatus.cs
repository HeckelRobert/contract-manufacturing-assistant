namespace QuotationAccelerator.Inbox.Domain;

public enum InboxMessageStatus
{
    New = 0,
    AutoReplied = 1,
    Escalated = 2,
    ContinuedToInquiry = 3,
    Replied = 4,
}
