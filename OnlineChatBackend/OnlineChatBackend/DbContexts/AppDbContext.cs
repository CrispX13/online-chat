using Microsoft.EntityFrameworkCore;
using OnlineChatBackend.Models;
using System.Reflection.Emit;

namespace OnlineChatBackend.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DbSet<Dialog> Dialogs { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);
            b.UseSerialColumns();

            b.Entity<Dialog>()
                .HasOne(d => d.FirstUser)
                .WithMany(c => c.DialogsAsFirstUser)
                .HasForeignKey(d => d.FirstUserId)
                .OnDelete(DeleteBehavior.Restrict);
            b.Entity<Dialog>()
               .HasOne(d => d.SecondUser)
               .WithMany(c => c.DialogsAsSecondUser)
               .HasForeignKey(d => d.SecondUserId)
               .OnDelete(DeleteBehavior.Restrict);

            b.Entity<Dialog>()
                .HasIndex(d => new { d.FirstUserId, d.SecondUserId })
                .IsUnique();

            b.Entity<Message>()
                .HasOne(d => d.Dialog)
                .WithMany(m => m.Messages)
                .HasForeignKey(d => d.DialogId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Entity<Notification>()
                .HasKey(n => new { n.DialogId, n.UserId })
                .HasName("PK_Notifications") 
                ;

            b.Entity<Notification>() 
                .ToTable("Notifications")
                .HasIndex(n => n.UserId);

            b.Entity<Notification>()
               .HasOne(n => n.Dialog)
               .WithMany(d => d.Notifications)
               .HasForeignKey(n => n.DialogId)
               .OnDelete(DeleteBehavior.Cascade);

            b.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(c => c.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
