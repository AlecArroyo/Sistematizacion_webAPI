using Sistematizacion_webAPI.Models;
using Sistematizacion_webAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Sistematizacion_webAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstudiantesController : ControllerBase
    {
        private readonly EstudianteService _estudianteService;

        public EstudiantesController(EstudianteService estudianteService)
        {
            _estudianteService = estudianteService;
        }

        // GET: api/estudiantes
        // Devuelve la lista completa de estudiantes registrados en la colección
        [HttpGet]
        public async Task<ActionResult<List<Estudiante>>> GetAll() =>
            await _estudianteService.GetAllAsync();

        // GET: api/estudiantes/{id}
        // Busca y devuelve un estudiante específico según su Id (ObjectId de Mongo)
        // Si no existe, responde 404 Not Found
        [HttpGet("{id}")]
        public async Task<ActionResult<Estudiante>> GetById(string id)
        {
            var estudiante = await _estudianteService.GetByIdAsync(id);
            if (estudiante == null) return NotFound();
            return estudiante;
        }

        // POST: api/estudiantes
        // Recibe un nuevo estudiante en el body (JSON) y lo inserta en la colección
        // Responde 201 Created junto con la ubicación del nuevo recurso
        [HttpPost]
        public async Task<IActionResult> Create(Estudiante nuevoEstudiante)
        {
            await _estudianteService.CreateAsync(nuevoEstudiante);
            return CreatedAtAction(nameof(GetById), new { id = nuevoEstudiante.Id }, nuevoEstudiante);
        }

        // PUT: api/estudiantes/{id}
        // Actualiza los datos de un estudiante existente, reemplazando el documento completo
        // Si el id no existe, responde 404 Not Found; si todo sale bien, responde 204 No Content
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Estudiante estudianteActualizado)
        {
            var existente = await _estudianteService.GetByIdAsync(id);
            if (existente == null) return NotFound();

            await _estudianteService.UpdateAsync(id, estudianteActualizado);
            return NoContent();
        }

        // DELETE: api/estudiantes/{id}
        // Elimina un estudiante según su Id
        // Si el id no existe, responde 404 Not Found; si se elimina correctamente, responde 204 No Content
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existente = await _estudianteService.GetByIdAsync(id);
            if (existente == null) return NotFound();

            await _estudianteService.DeleteAsync(id);
            return NoContent();
        }

        // PATCH: api/estudiantes/{id}/activar
        // Marca al estudiante como activo (isActive = true)
        [HttpPatch("{id}/activar")]
        public async Task<IActionResult> Activar(string id)
        {
            var existente = await _estudianteService.GetByIdAsync(id);
            if (existente == null) return NotFound();

            await _estudianteService.SetActiveStatusAsync(id, true);
            return NoContent();
        }

        // PATCH: api/estudiantes/{id}/desactivar
        // Marca al estudiante como inactivo (isActive = false)
        [HttpPatch("{id}/desactivar")]
        public async Task<IActionResult> Desactivar(string id)
        {
            var existente = await _estudianteService.GetByIdAsync(id);
            if (existente == null) return NotFound();

            await _estudianteService.SetActiveStatusAsync(id, false);
            return NoContent();
        }
    }
}