using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using Sistematizacion_webAPI.Models;
using Sistematizacion_webAPI.Services;

namespace Sistematizacion_webAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigEmailController : ControllerBase
    {
        private readonly ConfigEmailService _service;
        public record SendEmailsRequest(bool SendEmails);

        public ConfigEmailController(ConfigEmailService service)
        {
            _service = service;
        }

        // GET api/configemail
        [HttpGet]
        public async Task<ActionResult<ConfigEmail>> Get()
        {
            return Ok(await _service.GetAsync());
        }

        // PUT api/configemail
        [HttpPut]
        public async Task<ActionResult> Update(ConfigEmail config)
        {
            config.CorreoProfesora = (config.CorreoProfesora ?? "").Trim();

            if (config.CorreoProfesora != "" && !MailAddress.TryCreate(config.CorreoProfesora, out _))
                return BadRequest(new { mensaje = "El correo de la profesora no es válido." });

            if (config.EnviarCorreos && config.CorreoProfesora == "")
                return BadRequest(new { mensaje = "Para activar los envíos hace falta el correo de la profesora." });

            await _service.SaveAsync(config);
            return NoContent();
        }

        // GET api/configemail/send-emails
        [HttpGet("send-emails")]
        public async Task<ActionResult> GetSendEmails()
        {
            var cfg = await _service.GetAsync();
            return Ok(new { sendEmails = cfg.EnviarCorreos });
        }

        // PUT api/configemail/send-emails
        [HttpPut("send-emails")]
        public async Task<ActionResult> SetSendEmails(SendEmailsRequest request)
        {
            // No se puede activar el envío si no hay a quién avisarle
            if (request.SendEmails)
            {
                var cfg = await _service.GetAsync();
                if (string.IsNullOrWhiteSpace(cfg.CorreoProfesora))
                    return BadRequest(new { mensaje = "Para activar los envíos hace falta el correo de la profesora." });
            }

            await _service.SetSendEmailsAsync(request.SendEmails);
            return Ok(new { sendEmails = request.SendEmails });
        }

    }
}