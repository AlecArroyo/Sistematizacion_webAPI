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

        public SistematizacionController(SistematizacionService service)
        {
            _service = service;
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
    }
}