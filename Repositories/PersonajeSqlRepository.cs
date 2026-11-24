using DSW_3P_PR02.Data;
using DSW_3P_PR02.Models;

namespace DSW_3P_PR02.Repositories;

public class PersonajeSqlRepository : IPersonajeRepository
{
    private readonly AppDbContext _context;

    public PersonajeSqlRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<Personaje> ObtenerTodos()
    {
        return _context.Personajes.OrderBy(p => p.Id).ToList();
    }

    public Personaje? ObtenerPorId(int id)
    {
        return _context.Personajes.Find(id);
    }

    public List<Personaje> ObtenerPorVideojuego(string videojuego)
    {
        // La intercalación por defecto de SQL Server no distingue mayúsculas
        return _context.Personajes
            .Where(p => p.Videojuego == videojuego)
            .OrderBy(p => p.Id)
            .ToList();
    }

    public List<Personaje> ObtenerPorNivelMinimo(int nivel)
    {
        return _context.Personajes
            .Where(p => p.Nivel >= nivel)
            .OrderBy(p => p.Id)
            .ToList();
    }

    public Personaje Agregar(Personaje nuevo)
    {
        nuevo.Id = 0; // El Id lo genera SQL Server (IDENTITY)
        _context.Personajes.Add(nuevo);
        _context.SaveChanges();
        return nuevo;
    }

    public bool Actualizar(int id, Personaje personaje)
    {
        var actual = _context.Personajes.Find(id);
        if (actual == null)
            return false;

        actual.Nombre = personaje.Nombre;
        actual.Videojuego = personaje.Videojuego;
        actual.Clase = personaje.Clase;
        actual.Nivel = personaje.Nivel;
        actual.Poder = personaje.Poder;
        actual.Disponible = personaje.Disponible;
        actual.ImagenUrl = personaje.ImagenUrl;

        _context.SaveChanges();
        return true;
    }

    public bool Eliminar(int id)
    {
        var personaje = _context.Personajes.Find(id);
        if (personaje == null)
            return false;

        _context.Personajes.Remove(personaje);
        _context.SaveChanges();
        return true;
    }
}
