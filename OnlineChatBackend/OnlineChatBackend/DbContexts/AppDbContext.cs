using Microsoft.EntityFrameworkCore;
using OnlineChatBackend.Models;

namespace OnlineChatBackend.DbContexts;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Message> Messages { get; set; }

    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatParticipant> ChatParticipants { get; set; }

    public DbSet<Notification> Notifications { get; set; }

    // Новое:
    public DbSet<SearchQuery> SearchQueries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.UseSerialColumns(); // PostgreSQL [file:37]

        // Chat (Direct пара уникальна)
        modelBuilder.Entity<Chat>()
            .HasIndex(c => new { c.FirstUserId, c.SecondUserId })
            .IsUnique()
            .HasFilter("\"FirstUserId\" IS NOT NULL AND \"SecondUserId\" IS NOT NULL");

        // ChatParticipant ↔ Chat / Contact
        modelBuilder.Entity<ChatParticipant>()
            .HasOne(cp => cp.Chat)
            .WithMany(c => c.Participants)
            .HasForeignKey(cp => cp.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChatParticipant>()
            .HasOne(cp => cp.User)
            .WithMany(u => u.ChatParticipants)
            .HasForeignKey(cp => cp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ChatParticipant>()
            .HasIndex(cp => new { cp.ChatId, cp.UserId })
            .IsUnique();

        // Message ↔ Chat
        modelBuilder.Entity<Message>()
            .HasOne(m => m.Chat)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        // SearchQuery ↔ Contact
        modelBuilder.Entity<SearchQuery>()
            .HasOne(sq => sq.User)
            .WithMany(c => c.SearchQueries)
            .HasForeignKey(sq => sq.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // SearchQuery ↔ Chat
        modelBuilder.Entity<SearchQuery>()
            .HasOne(sq => sq.Chat)
            .WithMany() // можно сделать Chat.SearchQueries, если нужно
            .HasForeignKey(sq => sq.ChatId)
            .OnDelete(DeleteBehavior.Cascade);

        // Message ↔ SearchQuery (много сообщений на один запрос)
        modelBuilder.Entity<Message>()
            .HasOne(m => m.SearchQuery)
            .WithMany(sq => sq.Messages)
            .HasForeignKey(m => m.SearchQueryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Notification составной ключ
        modelBuilder.Entity<Notification>()
            .HasKey(n => new { n.ChatId, n.UserId });

        modelBuilder.Entity<Notification>()
            .ToTable("Notifications");

        modelBuilder.Entity<Notification>()
            .HasIndex(n => n.UserId);

        modelBuilder.Entity<Notification>()
           .HasOne(n => n.Chat)
           .WithMany(c => c.Notifications)
           .HasForeignKey(n => n.ChatId)
           .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}