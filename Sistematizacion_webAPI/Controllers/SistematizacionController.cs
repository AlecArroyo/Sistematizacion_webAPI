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
        private readonly ConfigEmailService _configEmailService;
        private readonly IConfiguration _config;
        private readonly ILogger<SistematizacionController> _log;

        private record ResultadoNotificacion(
            List<string> Enviados,
            List<string> Fallidos,
            List<string> Omitidos,
            List<string> SinAsignar);

        public SistematizacionController(
            SistematizacionService service,
            EstudianteService estudianteService,
            CorreoService correo,
            ConfigEmailService configEmailService,
            IConfiguration config,
            ILogger<SistematizacionController> log)
        {
            _service = service;
            _estudianteService = estudianteService;
            _correo = correo;
            _configEmailService = configEmailService;
            _config = config;
            _log = log;
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

            // La revisión del interruptor ocurre dentro del método
            _ = Task.Run(() => NotificarProfesoraAsync(sistematizacion));

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

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];

                if (!string.IsNullOrEmpty(item.PropietarioId))
                    ids.Add(item.PropietarioId);
                else
                    sinAsignar.Add(item.Propietario);
            }

            var estudiantes = await _estudianteService.GetByIdsAsync(ids);
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

        // POST api/sistematizacion/{id}/notificar
        [HttpPost("{id}/notificar")]
        public async Task<ActionResult> Notificar(string id)
        {
            var cfg = await LeerConfigAsync();

            if (!cfg.EnviarCorreos)
                return Conflict(new { mensaje = "El envío de correos está desactivado." });

            var gira = await _service.GetByIdAsync(id);
            if (gira is null) return NotFound();

            var resultado = await NotificarPersonasConMaterialAsync(gira, cfg);
            return Ok(resultado);
        }

        // Lee la configuración de Mongo y aplica el seguro local
        private async Task<ConfigEmail> LeerConfigAsync()
        {
            var cfg = await _configEmailService.GetAsync();

            // Seguro local: si Correo:Habilitado es false, nada se envía,
            // sin importar lo que diga la base de datos
            if (!_config.GetValue<bool>("Correo:Habilitado", true))
                cfg.EnviarCorreos = false;

            return cfg;
        }

        // Correo a la profesora cuando se sube una sistematización
        private async Task NotificarProfesoraAsync(Sistematizacion s)
        {
            try
            {
                var cfg = await LeerConfigAsync();
                if (!cfg.EnviarCorreos) return;

                if (string.IsNullOrWhiteSpace(cfg.CorreoProfesora))
                {
                    _log.LogWarning("Envío activo pero no hay correo de la profesora en la configuración");
                    return;
                }

                var html = PlantillaCorreo.NuevaSistematizacion(s);
                await _correo.EnviarAsync("Profesora TCU-782", cfg.CorreoProfesora,
                    $"Nueva sistematización: {s.Actividad}", html);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "No se pudo notificar a la profesora");
            }
        }

        // Correo a las personas con material audiovisual de una gira
        private async Task<ResultadoNotificacion> NotificarPersonasConMaterialAsync(Sistematizacion s, ConfigEmail cfg)
        {
            var enviados = new List<string>();
            var fallidos = new List<string>();
            var omitidos = new List<string>();
            var sinAsignar = new HashSet<string>();
            var ids = new HashSet<string>();

            var items = s.AudiosVideos?.Items;
            if (items is not null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    if (!string.IsNullOrEmpty(items[i].PropietarioId))
                        ids.Add(items[i].PropietarioId!);
                    else
                        sinAsignar.Add(items[i].Propietario);
                }
            }

            var estudiantes = await _estudianteService.GetByIdsAsync(ids);
            var link = cfg.LinkDrive;                               // <- ahora viene de Mongo
            var destinoPrueba = _config["Correo:DestinoPrueba"];    // sigue siendo solo local

            for (int i = 0; i < estudiantes.Count; i++)
            {
                var e = estudiantes[i];
                var nombre = $"{e.Nombre} {e.Apellido1} {e.Apellido2}".Trim();

                if (!e.IsActive)
                {
                    omitidos.Add(nombre);
                    continue;
                }

                var destino = string.IsNullOrWhiteSpace(destinoPrueba) ? e.Email : destinoPrueba;
                var html = PlantillaCorreo.MaterialGira(nombre, s.Actividad, link);
                var ok = await _correo.EnviarAsync(nombre, destino,
                    $"Material audiovisual de la gira {s.Actividad}", html);

                if (ok) enviados.Add(nombre);
                else fallidos.Add(nombre);
            }

            return new ResultadoNotificacion(enviados, fallidos, omitidos, sinAsignar.ToList());
        }
    }
}