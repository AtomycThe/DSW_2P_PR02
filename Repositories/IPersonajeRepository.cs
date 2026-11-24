using DSW_3P_PR02.Models;

namespace DSW_3P_PR02.Repositories;

public interface IPersonajeRepository
{
    List<Personaje> ObtenerTodos();
    Personaje? ObtenerPorId(int id);
    List<Personaje> ObtenerPorVideojuego(string videojuego);
    List<Personaje> ObtenerPorNivelMinimo(int nivel);
    Personaje Agregar(Personaje nuevo);
    bool Actualizar(int id, Personaje personaje);
    bool Eliminar(int id);
}
