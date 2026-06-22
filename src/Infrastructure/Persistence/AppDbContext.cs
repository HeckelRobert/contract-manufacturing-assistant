namespace QuotationAccelerator.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<ProjectIndexEntity> Projects => Set<ProjectIndexEntity>();

    public DbSet<AppSettingEntity> Settings => Set<AppSettingEntity>();

    public DbSet<MailAccountSettingsEntity> MailAccountSettings => Set<MailAccountSettingsEntity>();

    public DbSet<InboxMessageEntity> InboxMessages => Set<InboxMessageEntity>();

    public DbSet<InboxAttachmentEntity> InboxAttachments => Set<InboxAttachmentEntity>();

    public DbSet<SupportTicketEntity> SupportTickets => Set<SupportTicketEntity>();

    public DbSet<FaqTemplateEntity> FaqTemplates => Set<FaqTemplateEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProjectIndexEntity>(entity =>
        {
            entity.ToTable("projects");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.ProjectNumber);
        });

        modelBuilder.Entity<AppSettingEntity>(entity =>
        {
            entity.ToTable("settings");
            entity.HasKey(x => x.Key);
        });

        modelBuilder.Entity<MailAccountSettingsEntity>(entity =>
        {
            entity.ToTable("mail_account_settings");
            entity.HasKey(x => x.Id);
        });

        modelBuilder.Entity<InboxMessageEntity>(entity =>
        {
            entity.ToTable("inbox_messages");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.GraphMessageId).IsUnique();
            entity.HasMany(x => x.Attachments)
                .WithOne(x => x.Message)
                .HasForeignKey(x => x.InboxMessageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InboxAttachmentEntity>(entity =>
        {
            entity.ToTable("inbox_attachments");
            entity.HasKey(x => x.Id);
        });

        modelBuilder.Entity<SupportTicketEntity>(entity =>
        {
            entity.ToTable("support_tickets");
            entity.HasKey(x => x.Id);
            entity.HasIndex(x => x.InboxMessageId);
        });

        modelBuilder.Entity<FaqTemplateEntity>(entity =>
        {
            entity.ToTable("faq_templates");
            entity.HasKey(x => x.Id);
        });
    }
}
