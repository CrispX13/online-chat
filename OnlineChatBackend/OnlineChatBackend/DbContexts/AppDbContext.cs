using Microsoft.EntityFrameworkCore;
using OnlineChatBackend.Models;

namespace OnlineChatBackend.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatParticipant> ChatParticipants { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // PostgreSQL: генерация ключей (можно UseSerialColumns или UseIdentityByDefaultColumns)[web:42][web:51]
            modelBuilder.UseSerialColumns();

            // Ограничение: для Direct‑чатов пара FirstUserId/SecondUserId уникальна
            modelBuilder.Entity<Chat>()
                .HasIndex(c => new { c.FirstUserId, c.SecondUserId })
                .IsUnique()
                .HasFilter("\"FirstUserId\" IS NOT NULL AND \"SecondUserId\" IS NOT NULL");

            // ChatParticipant ↔ Chat / Contact (many-to-many)
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

            // Message ↔ Chat (1 ко многим)
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Chat)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);

            // Notification: составной ключ (ChatId + UserId)
            modelBuilder.Entity<Notification>()
                .HasKey(n => new { n.ChatId, n.UserId });

            modelBuilder.Entity<Notification>()
                .ToTable("Notifications");

            modelBuilder.Entity<Notification>()
                .HasIndex(n => n.UserId);

            // Notification ↔ Chat
            modelBuilder.Entity<Notification>()
               .HasOne(n => n.Chat)
               .WithMany(c => c.Notifications)
               .HasForeignKey(n => n.ChatId)
               .OnDelete(DeleteBehavior.Cascade);

            // Notification ↔ Contact
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}