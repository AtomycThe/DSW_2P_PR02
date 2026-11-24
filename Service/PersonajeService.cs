using DSW_3P_PR02.Models;
using DSW_3P_PR02.Repositories;

namespace DSW_3P_PR02.Services;

public class PersonajeService : IPersonajeService
{
    private readonly IPersonajeRepository _repository;

    public PersonajeService(IPersonajeRepository repository)
    {
        _repository = repository;
    }

    // Reglas de negocio
    private static void Validar(Personaje personaje)
    {
        if (string.IsNullOrWhiteSpace(personaje.Nombre))
            throw new ArgumentException("El nombre es obligatorio");

        if (string.IsNullOrWhiteSpace(personaje.Videojuego))
            throw new ArgumentException("El videojuego es obligatorio");

        if (string.IsNullOrWhiteSpace(personaje.Clase))
            throw new ArgumentException("La clase es obligatoria");

        if (personaje.Nivel < 1 || personaje.Nivel > 100)
            throw new ArgumentException("El nivel debe estar entre 1 y 100");

        if (personaje.Poder <= 0)
            throw new ArgumentException("El poder debe ser mayor que cero");

        bool urlValida = Uri.TryCreate(personaje.ImagenUrl, UriKind.Absolute, out var uri)
                         && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);

        if (!urlValida)
            throw new ArgumentException("ImagenUrl debe ser una URL absoluta válida (http o https)");

        personaje.Nombre = personaje.Nombre.Trim();
        personaje.Videojuego = personaje.Videojuego.Trim();
        personaje.Clase = personaje.Clase.Trim();
        personaje.ImagenUrl = personaje.ImagenUrl.Trim();
    }

    public List<Personaje> ObtenerTodos()
    {
        return _repository.ObtenerTodos();
    }

    public Personaje? ObtenerPorId(int id)
    {
        return _repository.ObtenerPorId(id);
    }

    public List<Personaje> ObtenerPorVideojuego(string videojuego)
    {
        if (string.IsNullOrWhiteSpace(videojuego))
            throw new ArgumentException("El nombre del videojuego es obligatorio");

        return _repository.ObtenerPorVideojuego(videojuego.Trim());
    }

    public List<Personaje> ObtenerPorNivelMinimo(int nivel)
    {
        if (nivel < 1 || nivel > 100)
            throw new ArgumentException("El nivel debe estar entre 1 y 100");

        return _repository.ObtenerPorNivelMinimo(nivel);
    }

    public Personaje Agregar(Personaje nuevo)
    {
        Validar(nuevo);
        nuevo.Disponible = true; // Los personajes nuevos se registran disponibles
        return _repository.Agregar(nuevo);
    }

    public Personaje? Actualizar(int id, Personaje personaje)
    {
        Validar(personaje);

        if (!_repository.Actualizar(id, personaje))
            return null;

        return _repository.ObtenerPorId(id);
    }

    public bool Eliminar(int id)
    {
        if (_repository.ObtenerPorId(id) == null)
            return false;

        return _repository.Eliminar(id);
    }
}
