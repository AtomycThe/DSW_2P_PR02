using Microsoft.EntityFrameworkCore;
using DSW_3P_PR02.Models;

namespace DSW_3P_PR02.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Personaje> Personajes => Set<Personaje>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Personaje>(entidad =>
        {
            entidad.ToTable("Personajes");
            entidad.HasKey(p => p.Id);
            entidad.Property(p => p.Nombre).IsRequired().HasMaxLength(100);
            entidad.Property(p => p.Videojuego).IsRequired().HasMaxLength(100);
            entidad.Property(p => p.Clase).IsRequired().HasMaxLength(50);
            entidad.Property(p => p.ImagenUrl).IsRequired().HasMaxLength(500);
            entidad.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Personajes_Nivel", "[Nivel] BETWEEN 1 AND 100");
                t.HasCheckConstraint("CK_Personajes_Poder", "[Poder] > 0");
            });

            // Datos iniciales
            entidad.HasData(
                new Personaje { Id = 1, Nombre = "Kratos", Videojuego = "God of War", Clase = "Guerrero", Nivel = 95, Poder = 9800, Disponible = true, ImagenUrl = "https://placehold.co/400x400/png?text=Kratos" },
                new Personaje { Id = 2, Nombre = "Link", Videojuego = "The Legend of Zelda", Clase = "Espadachin", Nivel = 80, Poder = 7500, Disponible = true, ImagenUrl = "https://placehold.co/400x400/png?text=Link" },
                new Personaje { Id = 3, Nombre = "Geralt", Videojuego = "The Witcher 3", Clase = "Brujo", Nivel = 70, Poder = 6900, Disponible = true, ImagenUrl = "https://placehold.co/400x400/png?text=Geralt" }
            );
        });
    }
}
