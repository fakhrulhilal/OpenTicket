using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace OpenTicket.Data.Entity
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Set by EF core")]
    public class OpenTicketDbContext : DbContext
    {
        private const string DefaultConnectionString = "Data Source=(local);Initial Catalog=OpenTicket;Integrated Security=true;";
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
        public virtual DbSet<ExternalAccount> ExternalAccounts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder.UseSqlServer(_connectionString ?? DefaultConnectionString);

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
                entity.Property(e => e.ExternalAccountId)
                    .HasColumnName("ExternalAccountId");
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
                entity.HasOne(e => e.ExternalAccount)
                    .WithMany(e => e.EmailAccounts)
                    .HasForeignKey(e => e.ExternalAccountId)
                    .HasConstraintName("FK_EmailAccount_Provider");
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

            modelBuilder.Entity<ExternalAccount>(entity =>
            {
                entity.ToTable("ExternalAccounts");
                entity.Property(e => e.Protocol)
                    .HasColumnName("Protocol")
                    .IsRequired()
                    .HasConversion<short>();
                entity.Property(e => e.Name)
                    .HasColumnName("Name")
                    .IsRequired()
                    .HasMaxLength(50);
                entity.Property(e => e.Identifier)
                    .HasColumnName("TenantIdentifier")
                    .IsRequired()
                    .HasMaxLength(250);
                entity.Property(e => e.ClientId)
                    .HasColumnName("ClientId")
                    .IsRequired();
                entity.Property(e => e.Secret)
                    .HasColumnName("Secret")
                    .IsRequired();
            });
        }
    }
}
