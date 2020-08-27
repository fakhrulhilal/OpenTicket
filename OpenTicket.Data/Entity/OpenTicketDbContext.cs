using Microsoft.EntityFrameworkCore;

namespace OpenTicket.Data.Entity
{
    public class OpenTicketDbContext : DbContext
    {
        public OpenTicketDbContext()
        { }

        public OpenTicketDbContext(DbContextOptions<OpenTicketDbContext> options)
            : base(options)
        { }

        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<EmailAccount> EmailAccounts { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }

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
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(e => e.LastUpdateAccessToken).HasColumnType("datetime");

                entity.Property(e => e.MailBox).HasMaxLength(255);

                entity.Property(e => e.ServerAddress)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasMaxLength(50);
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
