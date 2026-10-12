using System.Net;

namespace Sistematizacion_webAPI.Services
{
    public static class PlantillaCorreo
    {
        public static string MaterialGira(string nombre, string gira, string link) => $@"
            <p>¡Hola, {WebUtility.HtmlEncode(nombre)}!</p>
            <p>Según el registro de la gira <strong>{WebUtility.HtmlEncode(gira)}</strong>,
               tienes material audiovisual de la actividad.</p>
            <p>Por favor súbelo al Drive en este enlace:<br>
               <a href=""{link}"">{link}</a></p>
            <p>¡Gracias!<br>TCU-782</p>";
    }
}