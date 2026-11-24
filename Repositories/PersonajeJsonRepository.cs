using System.Text.Json;
using DSW_3P_PR02.Models;

namespace DSW_3P_PR02.Repositories;

public class PersonajeJsonRepository : IPersonajeRepository
{
    private readonly string _ruta;
    private static readonly object _candado = new();
    private readonly JsonSerializerOptions _opciones = new()
    {
        WriteIndented = true
    };

    public PersonajeJsonRepository(IWebHostEnvironment environment)
    {
        _ruta = Path.Combine(environment.ContentRootPath, "Data", "personajes.json");
        Directory.CreateDirectory(Path.GetDirectoryName(_ruta)!);

        if (!File.Exists(_ruta))
            File.WriteAllText(_ruta, "[]");
    }

    private List<Personaje> Leer()
    {
        lock (_candado)
        {
            string json = File.ReadAllText(_ruta);
            return JsonSerializer.Deserialize<List<Personaje>>(json) ?? new List<Personaje>();
        }
    }

    private void Guardar(List<Personaje> datos)
    {
        lock (_candado)
        {
            string json = JsonSerializer.Serialize(datos, _opciones);
            File.WriteAllText(_ruta, json);
        }
    }

    public List<Personaje> ObtenerTodos()
    {
        return Leer();
    }

    public Personaje? ObtenerPorId(int id)
    {
        return Leer().FirstOrDefault(p => p.Id == id);
    }

    public List<Personaje> ObtenerPorVideojuego(string videojuego)
    {
        return Leer()
            .Where(p => p.Videojuego.Equals(videojuego, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<Personaje> ObtenerPorNivelMinimo(int nivel)
    {
        return Leer().Where(p => p.Nivel >= nivel).ToList();
    }

    public Personaje Agregar(Personaje nuevo)
    {
        var datos = Leer();
        nuevo.Id = datos.Count == 0 ? 1 : datos.Max(p => p.Id) + 1;
        datos.Add(nuevo);
        Guardar(datos);
        return nuevo;
    }

    public bool Actualizar(int id, Personaje personaje)
    {
        var datos = Leer();
        var existente = datos.FirstOrDefault(p => p.Id == id);

        if (existente == null)
            return false;

        existente.Nombre = personaje.Nombre;
        existente.Videojuego = personaje.Videojuego;
        existente.Clase = personaje.Clase;
        existente.Nivel = personaje.Nivel;
        existente.Poder = personaje.Poder;
        existente.Disponible = personaje.Disponible;
        existente.ImagenUrl = personaje.ImagenUrl;

        Guardar(datos);
        return true;
    }

    public bool Eliminar(int id)
    {
        var datos = Leer();
        var existente = datos.FirstOrDefault(p => p.Id == id);

        if (existente == null)
            return false;

        datos.Remove(existente);
        Guardar(datos);
        return true;
    }
}
