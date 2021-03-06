﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace OpenTicket.Data.Entity
{
    public class OpenTicketDbContext : DbContext
    {
        private readonly string _connectionString;

        public OpenTicketDbContext()
        { }

        /// <summary>
        /// Use SQL server with specified connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public OpenTicketDbContext(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString)) throw new ArgumentNullException(nameof(connectionString));
            _connectionString = connectionString;
        }

        public OpenTicketDbContext(DbContextOptions<OpenTicketDbContext> options)
            : base(options)
        { }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<EmailAccount> EmailAccounts { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrWhiteSpace(_connectionString))
                optionsBuilder.UseSqlServer(_connectionString);
            else
                optionsBuilder.UseSqlServer(
                    "Data Source=(local);Initial Catalog=OpenTicket;Integrated Security=true;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");
                entity.Property(e => e.DisplayName).HasMaxLength(250);
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(250);
                entity.Property(e => e.RegisteredDateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<EmailAccount>(entity =>
            {
                entity.ToTable("EmailAccounts");
                entity.Property(e => e.IsActive)
                    .HasColumnName("IsActive")
                    .IsRequired()
                    .HasDefaultValue(true);
                entity.Property(e => e.DraftId)
                    .HasColumnName("DraftId");
                entity.Property(e => e.Email)
                    .HasColumnName("Email")
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.LastUpdateAccessToken)
                    .HasColumnName("LastUpdateAccessToken")
                    .HasColumnType("datetime");
                entity.Property(e => e.MailBox).HasColumnName("MailBox").HasMaxLength(255);
                entity.Property(e => e.ServerAddress)
                    .HasColumnName("ServerAddress")
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Username)
                    .HasColumnName("Username")
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Protocol)
                    .HasColumnName("Protocol")
                    .IsRequired()
                    .HasConversion<short>();
                entity.Property(e => e.AccessToken)
                    .HasColumnName("AccessToken");
                entity.Property(e => e.RefreshToken)
                    .HasColumnName("RefreshToken");
            });

            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.ToTable("Tickets");
                entity.Property(e => e.CreatedDateTime).HasColumnType("datetime");
                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(250);
                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ticket_Owner");
                entity.HasOne(d => d.EmailAccount)
                    .WithMany(p => p.Tickets)
                    .HasForeignKey(d => d.EmailAccountId)
                    .HasConstraintName("FK_Ticket_ImportedEmail");
            });
        }
    }
}
