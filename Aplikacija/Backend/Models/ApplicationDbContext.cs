using Microsoft.EntityFrameworkCore;

namespace WebTemplate.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
    {
    }

    public DbSet<Korisnik> Korisnici { get; set; }
    public DbSet<Oglas> Oglasi { get; set; }
    public DbSet<Kategorija> Kategorije { get; set; }
    public DbSet<Prijava> Prijave { get; set; }
    public DbSet<Chat> Chatovi { get; set; }
    public DbSet<Poruka> Poruke { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Konfiguracija za Korisnik
        modelBuilder.Entity<Korisnik>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.Property(e => e.Ime).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Prezime).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);

            // Veza sa Oglasi
            entity.HasMany(e => e.Oglasi)
                  .WithOne(o => o.PostavljacOglasa)
                  .HasForeignKey(o => o.PostavljacOglasaId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Veza sa Prijave
            entity.HasMany(e => e.Prijave)
                  .WithOne(p => p.Korisnik)
                  .HasForeignKey(p => p.KorisnikId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Konfiguracija za Oglas
        modelBuilder.Entity<Oglas>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.Property(e => e.Naziv).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Opis).HasMaxLength(200);
            entity.Property(e => e.PostavljacOglasaId).IsRequired();

            // Veza sa Korisnik
            entity.HasOne(o => o.PostavljacOglasa)
                  .WithMany(k => k.Oglasi)
                  .HasForeignKey(o => o.PostavljacOglasaId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Veza sa Prijave
            entity.HasMany(o => o.Prijave)
                  .WithOne(p => p.Oglas)
                  .HasForeignKey(p => p.OglasId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Konfiguracija za Prijavu
        modelBuilder.Entity<Prijava>(entity =>
        {
            entity.HasKey(e => e.ID);
            entity.Property(e => e.KorisnikId).IsRequired();
            entity.Property(e => e.OglasId).IsRequired();
            entity.Property(e => e.VremePrijave).IsRequired();
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Na cekanju");
            entity.Property(e => e.Poruka).HasMaxLength(500);

            // Veza sa Korisnik
            entity.HasOne(p => p.Korisnik)
                  .WithMany(k => k.Prijave)
                  .HasForeignKey(p => p.KorisnikId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Veza sa Oglas
            entity.HasOne(p => p.Oglas)
                  .WithMany(o => o.Prijave)
                  .HasForeignKey(p => p.OglasId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(p => new { p.KorisnikId, p.OglasId }).IsUnique();
        });
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Veza sa Oglas
            entity.HasOne(e => e.Oglas)
                  .WithMany()
                  .HasForeignKey(e => e.OglasId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Veza sa Klijent (korisnik koji se prijavio)
            entity.HasOne(e => e.Klijent)
                  .WithMany()
                  .HasForeignKey(e => e.KlijentId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Veza sa Oglasivač (vlasnik oglasa)
            entity.HasOne(e => e.Oglasivac)
                  .WithMany()
                  .HasForeignKey(e => e.OglasivacId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Chat se kreira samo jednom za par (oglas + klijent)
            entity.HasIndex(e => new { e.OglasId, e.KlijentId }).IsUnique();
        });

        modelBuilder.Entity<Poruka>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Tekst).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.VremeSlanja).IsRequired();

            // Veza sa Chat
            entity.HasOne(e => e.Chat)
                  .WithMany(c => c.Poruke)
                  .HasForeignKey(e => e.ChatId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Veza sa Posiljalac
            entity.HasOne(e => e.Posiljalac)
                  .WithMany()
                  .HasForeignKey(e => e.PosiljalacId)
                  .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
