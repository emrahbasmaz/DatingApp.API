using DatingApp.Api.Models;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Values> Values { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }

        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Like>().HasKey(k => new { k.LikerId, k.LikeeId });
            builder.Entity<Like>()
                    .HasOne(u => u.Likee)
                    .WithMany(u => u.Liker)
                    .HasForeignKey(u => u.LikerId)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                  .HasOne(u => u.Liker)
                  .WithMany(u => u.Likee)
                  .HasForeignKey(u => u.LikeeId)
                  .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(p => p.Sender)
                .WithMany(p => p.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
              .HasOne(p => p.Recipient)
              .WithMany(p => p.MessagesReceived)
              .OnDelete(DeleteBehavior.Restrict);

        }
    }
}