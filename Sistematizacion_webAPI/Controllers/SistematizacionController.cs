using Sistematizacion_webAPI.Models;
using Sistematizacion_webAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Sistematizacion_webAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SistematizacionController : ControllerBase
    {
        private readonly SistematizacionService _service;

        private readonly EstudianteService _estudianteService;

        private readonly CorreoService _correo;


        public SistematizacionController(
            SistematizacionService service,
            EstudianteService estudianteService,
            CorreoService correo)
        {
            _service = service;
            _estudianteService = estudianteService;
            _correo = correo;
        }

        // GET api/sistematizacion
        [HttpGet]
        public async Task<ActionResult<List<Sistematizacion>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // GET api/sistematizacion/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Sistematizacion>> GetById(string id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result is null) return NotFound();
            return Ok(result);
        }

        // POST api/sistematizacion
        [HttpPost]
        public async Task<ActionResult> Create(Sistematizacion sistematizacion)
        {
            await _service.CreateAsync(sistematizacion);
            return CreatedAtAction(nameof(GetById), new { id = sistematizacion.Id }, sistematizacion);
        }

        // PUT api/sistematizacion/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, Sistematizacion sistematizacion)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing is null) return NotFound();

            sistematizacion.Id = id;
            await _service.UpdateAsync(id, sistematizacion);
            return NoContent();
        }

        // DELETE api/sistematizacion/{id}
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            var existing = await _service.GetByIdAsync(id);
            if (existing is null) return NotFound();

            await _service.DeleteAsync(id);
            return NoContent();
        }

        // GET api/sistematizacion/{id}/destinatarios-material
        [HttpGet("{id}/destinatarios-material")]
        public async Task<ActionResult> GetDestinatariosMaterial(string id)
        {
            var gira = await _service.GetByIdAsync(id);
            if (gira is null) return NotFound();

            var items = gira.AudiosVideos.Items;

            var ids = new HashSet<string>();
            var sinAsignar = new HashSet<string>();

            // Separar: los que tienen id y los que no
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                if (!string.IsNullOrEmpty(item.PropietarioId))
                    ids.Add(item.PropietarioId);
                else
                    sinAsignar.Add(item.Propietario);
            }

            // Buscar a esas personas en la colección de estudiantes
            var estudiantes = await _estudianteService.GetByIdsAsync(ids);

            // Armar la respuesta con nombre completo y correo
            var destinatarios = new List<object>();

            for (int i = 0; i < estudiantes.Count; i++)
            {
                var e = estudiantes[i];

                destinatarios.Add(new
                {
                    id = e.Id,
                    nombre = $"{e.Nombre} {e.Apellido1} {e.Apellido2}".Trim(),
                    email = e.Email,
                    activo = e.IsActive
                });
            }

            return Ok(new { gira = gira.Actividad, destinatarios, sinAsignar });
        }

        // TEMPORAL: borrar después de probar
        [HttpPost("probar-correo")]
        public async Task<ActionResult> ProbarCorreo([FromQuery] string para)
        {
            var html = "<p>Hola, si lees esto el envío funciona.</p>";
            var ok = await _correo.EnviarAsync("Prueba", para, "Prueba TCU-782", html);

            return ok ? Ok("Enviado") : StatusCode(500, "Falló, revisa el log");
        }



    }
}