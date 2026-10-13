using System.Net;
using System.Text;
using Sistematizacion_webAPI.Models;
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

        public static string NuevaSistematizacion(Sistematizacion s)
        {
            var items = s.AudiosVideos.Items;
            var lista = new StringBuilder();

            for (int i = 0; i < items.Count; i++)
            {
                lista.Append($"<li>{WebUtility.HtmlEncode(items[i].Titulo)} — {WebUtility.HtmlEncode(items[i].Propietario)}</li>");
            }

            var material = items.Count > 0
                ? $"<ul>{lista}</ul>"
                : "<p>No se registró material audiovisual.</p>";

            return $@"
               <p>{WebUtility.HtmlEncode(s.Notas)}</p>
               <p>Se registró una nueva sistematización.</p>
                 <p><strong>Actividad:</strong> {WebUtility.HtmlEncode(s.Actividad)}<br>
                <strong>Registrada por:</strong> {WebUtility.HtmlEncode(s.Nombre)}<br>
                  <strong>Comunidad:</strong> {WebUtility.HtmlEncode(s.Comunidad)}<br>
              <strong>Distrito:</strong> {WebUtility.HtmlEncode(s.Distrito)}<br>
               <strong>Participantes:</strong> {s.Participantes}<br>
               <strong>Horas:</strong> {s.Horas}</p>
               <p><strong>Material audiovisual:</strong></p>
                 {material}
                 <p>TCU-782</p>";
        }



    }




}