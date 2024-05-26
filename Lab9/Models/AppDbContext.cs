using Microsoft.EntityFrameworkCore;

namespace Lab9.Models
{
    public partial class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Trip> Trips { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<ClientTrip> ClientTrips { get; set; }
        public virtual DbSet<Country> Countries { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Your Connection String Here");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<Trip>(entity =>
            {
                entity.HasKey(e => e.IdTrip).HasName("Trip_pk");

                entity.ToTable("Trip");

                entity.Property(e => e.IdTrip).ValueGeneratedNever();
                entity.Property(e => e.DateFrom).HasColumnType("datetime");
                entity.Property(e => e.DateTo).HasColumnType("datetime");
                entity.Property(e => e.Description).HasMaxLength(220);
                entity.Property(e => e.Name).HasMaxLength(120);
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.IdClient).HasName("Client_pk");

                entity.ToTable("Client");

                entity.Property(e => e.IdClient).ValueGeneratedNever();
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Telephone).HasMaxLength(20);
                entity.Property(e => e.Pesel).HasMaxLength(11);
            });

            modelBuilder.Entity<ClientTrip>(entity =>
            {
                entity.HasKey(e => new { e.IdClient, e.IdTrip }).HasName("ClientTrip_pk");

                entity.ToTable("ClientTrip");

                entity.Property(e => e.RegisteredAt).HasColumnType("datetime");
                entity.Property(e => e.PaymentDate).HasColumnType("datetime");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientTrips)
                    .HasForeignKey(d => d.IdClient)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ClientTrip_Client");

                entity.HasOne(d => d.Trip)
                    .WithMany(p => p.ClientTrips)
                    .HasForeignKey(d => d.IdTrip)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ClientTrip_Trip");
            });

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasKey(e => e.IdCountry).HasName("Country_pk");

                entity.ToTable("Country");

                entity.Property(e => e.IdCountry).ValueGeneratedNever();
                entity.Property(e => e.Name).HasMaxLength(100);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

        public DbSet<Client> GetClients()
        {
            return Clients;
        }

        public DbSet<Trip> GetTrips()
        {
            return Trips;
        }

        public DbSet<ClientTrip> GetClientTrips()
        {
            return ClientTrips;
        }
    }
}
