using DSW_3P_PR02.Models;

namespace DSW_3P_PR02.Services;

public interface IPersonajeService
{
    List<Personaje> ObtenerTodos();
    Personaje? ObtenerPorId(int id);
    List<Personaje> ObtenerPorVideojuego(string videojuego);
    List<Personaje> ObtenerPorNivelMinimo(int nivel);
    Personaje Agregar(Personaje nuevo);
    Personaje? Actualizar(int id, Personaje personaje);
    bool Eliminar(int id);
}
