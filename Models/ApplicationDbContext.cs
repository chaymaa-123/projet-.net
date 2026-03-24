using Microsoft.EntityFrameworkCore;

namespace Readify.Models;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Achats_Utilisateur> Achats_Utilisateurs { get; set; }

    public virtual DbSet<Commande> Commandes { get; set; }

    public virtual DbSet<Details_Commande> Details_Commandes { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Livre> Livres { get; set; }

    public virtual DbSet<Role> Roles { get; set; }
    public DbSet<Panier> Paniers { get; set; }



    public virtual DbSet<Utilisateur> Utilisateurs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Readify;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<Achats_Utilisateur>(entity =>
        //{
        //    entity.HasKey(e => e.AchatID).HasName("PK__Achats_U__303B3DFA4F2DDCF1");

        //    entity.Property(e => e.DateAcquisition).HasDefaultValueSql("(getdate())");

        //    entity.HasOne(d => d.Commande).WithMany(p => p.Achats_Utilisateurs)
        //        .OnDelete(DeleteBehavior.ClientSetNull)
        //        .HasConstraintName("FK__Achats_Ut__Comma__01142BA1");

        //    entity.HasOne(d => d.Livre).WithMany(p => p.Achats_Utilisateurs)
        //        .OnDelete(DeleteBehavior.ClientSetNull)
        //        .HasConstraintName("FK__Achats_Ut__Livre__00200768");

        //    entity.HasOne(d => d.Utilisateur).WithMany(p => p.Achats_Utilisateurs)
        //        .OnDelete(DeleteBehavior.ClientSetNull)
        //        .HasConstraintName("FK__Achats_Ut__Utili__7F2BE32F");
        //});

        modelBuilder.Entity<Commande>(entity =>
        {
            entity.HasKey(e => e.CommandeID).HasName("PK__Commande__E8361F519A949FB1");

            entity.Property(e => e.DateCommande).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Utilisateur).WithMany(p => p.Commandes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Commandes__Utili__5AEE82B9");
        });

        modelBuilder.Entity<Details_Commande>(entity =>
        {
            entity.HasKey(e => e.DetailID).HasName("PK__Details___135C314DF7F71725");

            entity.HasOne(d => d.Commande).WithMany(p => p.Details_Commandes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Details_C__Comma__797309D9");

            entity.HasOne(d => d.Livre).WithMany(p => p.Details_Commandes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Details_C__Livre__7A672E12");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreID).HasName("PK__Genres__0385055EBAB824A2");
        });

        modelBuilder.Entity<Livre>(entity =>
        {
            entity.HasKey(e => e.LivreId).HasName("PK__Livre__562AE7875F0CCCB0");

            entity.Property(e => e.EstPubliee).HasDefaultValue(false);

            entity.HasOne(d => d.Genre).WithMany(p => p.Livres)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Livre__GenreID__75A278F5");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleID).HasName("PK__Roles__8AFACE3A3F766586");
        });

        modelBuilder.Entity<Utilisateur>(entity =>
        {
            entity.HasKey(e => e.UtilisateurID).HasName("PK__Utilisat__6CB6AE1F2E9E688E");

            entity.Property(e => e.DateInscription).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Role).WithMany(p => p.Utilisateurs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Utilisate__RoleI__52593CB8");
        });

        OnModelCreatingPartial(modelBuilder);
    }



    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
