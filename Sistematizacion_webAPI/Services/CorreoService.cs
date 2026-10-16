using System.Net;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Sistematizacion_webAPI.Services
{
    public class CorreoService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<CorreoService> _log;

        public CorreoService(IConfiguration config, ILogger<CorreoService> log)
        {
            _config = config;
            _log = log;
        }

        public async Task<bool> EnviarAsync(string paraNombre, string paraCorreo, string asunto, string html)
        {
            try
            {
                // 1. Armar el mensaje
                var mensaje = new MimeMessage();
                mensaje.From.Add(new MailboxAddress(_config["Correo:NombreRemitente"], _config["Correo:Usuario"]));
                mensaje.To.Add(new MailboxAddress(paraNombre, paraCorreo));
                mensaje.Subject = asunto;
                mensaje.Body = new BodyBuilder { HtmlBody = html }.ToMessageBody();

                // 2. Conectar con Gmail
                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(
                    _config["Correo:Host"],
                    int.Parse(_config["Correo:Puerto"]!),
                    SecureSocketOptions.StartTls);

                // 3. Identificarse con la contraseña de aplicación
                await smtp.AuthenticateAsync(_config["Correo:Usuario"], _config["Correo:Clave"]);

                // 4. Enviar y cerrar
                await smtp.SendAsync(mensaje);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "No se pudo enviar el correo a {Correo}", paraCorreo);
                return false;
            }
        }




    }
}