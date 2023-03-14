using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HydroApp
{

    public partial class HydropressUserDbContext : DbContext
    {
        string _connectionString;
        public HydropressUserDbContext(string connectionString
            = "Data Source=DESKTOP-8L2M3KV\\SQLEXPRESS01;Initial Catalog=HydropressDB;Integrated Security=True;MultipleActiveResultSets=True;TrustServerCertificate=True")
        {
            _connectionString = connectionString;
        }

        public HydropressUserDbContext(DbContextOptions<HydropressUserDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }

        public virtual DbSet<UserMessage> UserMessages { get; set; }

        public virtual DbSet<UserMessagesContent> UserMessagesContents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(_connectionString);
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK_UserId");

                entity.Property(e => e.Nickname).HasMaxLength(64);
                entity.Property(e => e.Password).HasMaxLength(128);
            });

            modelBuilder.Entity<UserMessage>(entity =>
            {
                entity.HasKey(e => e.UserMessageId).HasName("PK_User_MessagesId");

                entity.ToTable("User_Messages");

                entity.Property(e => e.UserMessageId).HasColumnName("User_MessageId");
                entity.Property(e => e.RedactedDate).HasColumnType("datetime");
                entity.Property(e => e.SendedDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.ReceiverNavigation).WithMany(p => p.UserMessageReceiverNavigations)
                    .HasForeignKey(d => d.Receiver)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Receiver_UserId");

                entity.HasOne(d => d.SenderNavigation).WithMany(p => p.UserMessageSenderNavigations)
                    .HasForeignKey(d => d.Sender)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Sender_UserId");
            });

            modelBuilder.Entity<UserMessagesContent>(entity =>
            {
                entity
                    .HasNoKey()
                    .ToTable("User_Messages_Content");

                entity.HasIndex(e => e.UserMessageId, "UQ__User_Mes__43009F37E7C70CDD").IsUnique();

                entity.Property(e => e.Content).HasColumnType("ntext");
                entity.Property(e => e.UserMessageId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("User_MessageId");

                entity.HasOne(d => d.UserMessage).WithOne()
                    .HasForeignKey<UserMessagesContent>(d => d.UserMessageId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserMessageContent_UserMessage");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}