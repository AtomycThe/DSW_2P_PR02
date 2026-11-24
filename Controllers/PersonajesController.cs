using DSW_3P_PR02.Models;
using DSW_3P_PR02.Services;
using Microsoft.AspNetCore.Mvc;

namespace DSW_3P_PR02.Controllers;

[ApiController]
[Route("api/personajes")]
[Produces("application/json")]
public class PersonajesController : ControllerBase
{
    private readonly IPersonajeService _service;

    public PersonajesController(IPersonajeService service)
    {
        _service = service;
    }

    [HttpGet]
    [EndpointSummary("Consultar todos los personajes")]
    [ProducesResponseType(typeof(List<Personaje>), StatusCodes.Status200OK)]
    public IActionResult ObtenerTodos() => Ok(_service.ObtenerTodos());

    [HttpGet("{id:int}")]
    [EndpointSummary("Consultar personaje por ID")]
    [ProducesResponseType(typeof(Personaje), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult ObtenerPorId(int id)
    {
        var personaje = _service.ObtenerPorId(id);
        return personaje == null
            ? NotFound(new { mensaje = $"No existe el personaje con id {id}" })
            : Ok(personaje);
    }

    [HttpGet("videojuego/{nombre}")]
    [EndpointSummary("Consultar personajes por videojuego")]
    [ProducesResponseType(typeof(List<Personaje>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult ObtenerPorVideojuego(string nombre)
    {
        try
        {
            return Ok(_service.ObtenerPorVideojuego(nombre));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpGet("nivel/{nivel:int}")]
    [EndpointSummary("Consultar personajes con nivel igual o superior")]
    [ProducesResponseType(typeof(List<Personaje>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult ObtenerPorNivel(int nivel)
    {
        try
        {
            return Ok(_service.ObtenerPorNivelMinimo(nivel));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPost]
    [EndpointSummary("Registrar personaje")]
    [ProducesResponseType(typeof(Personaje), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Agregar([FromBody] Personaje personaje)
    {
        try
        {
            var creado = _service.Agregar(personaje);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    [EndpointSummary("Actualizar personaje")]
    [ProducesResponseType(typeof(Personaje), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Actualizar(int id, [FromBody] Personaje personaje)
    {
        try
        {
            var actualizado = _service.Actualizar(id, personaje);
            return actualizado == null
                ? NotFound(new { mensaje = $"No existe el personaje con id {id}" })
                : Ok(actualizado);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    [EndpointSummary("Eliminar personaje")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Eliminar(int id)
    {
        return _service.Eliminar(id)
            ? NoContent()
            : NotFound(new { mensaje = $"No existe el personaje con id {id}" });
    }
}
