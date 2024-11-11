using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace F_Driver.DataAccessObject.Models;

public partial class FDriverContext : DbContext
{
    public FDriverContext()
    {
    }

    public FDriverContext(DbContextOptions<FDriverContext> options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Data Source=db.fjourney.site;Initial Catalog=F-Driver_ver2;User ID=SA;Password=<YourStrong@Passw0rda>;TrustServerCertificate=True");
        }
    }

    public virtual DbSet<Cancellation> Cancellations { get; set; }

    public virtual DbSet<CancellationReason> CancellationReasons { get; set; }

    public virtual DbSet<Driver> Drivers { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PriceTable> PriceTables { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TripMatch> TripMatches { get; set; }

    public virtual DbSet<TripRequest> TripRequests { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

    public virtual DbSet<Wallet> Wallets { get; set; }

    public virtual DbSet<Zone> Zones { get; set; }
    public virtual DbSet<Admin> Admins { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //}
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cancellation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cancella__6A2D9A3A6734385B");

            entity.Property(e => e.CanceledAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Reason).WithMany(p => p.Cancellations).HasConstraintName("FK__Cancellat__Reaso__01142BA1");

            entity.HasOne(d => d.TripMatch).WithMany(p => p.Cancellations).HasConstraintName("FK__Cancellat__TripM__00200768");
        });

        modelBuilder.Entity<CancellationReason>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cancella__A4F8C0E7BD34C5C7");
        });

        modelBuilder.Entity<Driver>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Drivers__F1B1CD049D52C77F");

            entity.Property(e => e.Verified).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithOne(p => p.Driver)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Drivers__UserId__4222D4EF");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Feedback__6A4BEDD6392927F6");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Driver).WithMany(p => p.Feedbacks).HasConstraintName("FK__Feedback__Driver__6C190EBB");

            entity.HasOne(d => d.Match).WithMany(p => p.Feedbacks).HasConstraintName("FK__Feedback__MatchI__6A30C649");

            entity.HasOne(d => d.Passenger).WithMany(p => p.Feedbacks).HasConstraintName("FK__Feedback__Passen__6B24EA82");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Messages__C87C0C9C71DCD5C1");

            entity.Property(e => e.SentAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Match).WithMany(p => p.Messages).HasConstraintName("FK__Messages__MatchI__5BE2A6F2");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages).HasConstraintName("FK__Messages__Sender__5CD6CB2B");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payments__9B556A387BED1B1C");

            entity.Property(e => e.PaidAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("pending");

            entity.HasOne(d => d.Driver).WithMany(p => p.PaymentsOfDriver).HasConstraintName("FK__Payments__Driver__656C112C");

            entity.HasOne(d => d.Match).WithMany(p => p.Payments).HasConstraintName("FK__Payments__MatchI__6383C8BA");

            entity.HasOne(d => d.Passenger).WithMany(p => p.PaymentsOfPassenger).HasConstraintName("FK__Payments__Passen__6477ECF3");
        });

        modelBuilder.Entity<PriceTable>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PriceTab__49575BAF1A1D0D0D");

            entity.HasOne(d => d.FromZone).WithMany(p => p.PriceTableFromZones)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PriceTabl__FromZ__6EF57B66");

            entity.HasOne(d => d.ToZone).WithMany(p => p.PriceTableToZones)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PriceTabl__ToZon__6FE99F9F");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transact__55433A6B4AC4DF18");

            entity.Property(e => e.TransactionDate).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Wallet).WithMany(p => p.Transactions).HasConstraintName("FK__Transacti__Walle__7A672E12");
        });

        modelBuilder.Entity<TripMatch>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripMatc__4218C817E65D171B");

            entity.Property(e => e.MatchedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("pending");

            entity.HasOne(d => d.Driver).WithMany(p => p.TripMatchesAsDriver);
            
        });

        modelBuilder.Entity<TripRequest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TripRequ__33A8517AEC13D92A");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("available");

            entity.HasOne(d => d.FromZone).WithMany(p => p.TripRequestFromZones)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TripReque__FromZ__5070F446");

            entity.HasOne(d => d.ToZone).WithMany(p => p.TripRequestToZones)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TripReque__ToZon__5165187F");

            entity.HasOne(d => d.User).WithMany(p => p.TripRequests)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__TripReque__UserI__4F7CD00D");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__1788CC4C5AA6AB44");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Verified).HasDefaultValue(false);
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vehicles__476B549262DB2AF0");

            entity.Property(e => e.IsVerified).HasDefaultValue(false);

            entity.HasOne(d => d.Driver).WithMany(p => p.Vehicles)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Vehicles__Driver__46E78A0C");
        });

        modelBuilder.Entity<Wallet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Wallets__84D4F90E8673D7BE");

            entity.Property(e => e.Balance).HasDefaultValue(0.00m);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithOne(p => p.Wallet)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Wallets__UserId__75A278F5"); 
        });

        modelBuilder.Entity<Zone>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Zones__601667B5715283B6");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
