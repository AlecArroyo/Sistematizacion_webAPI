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

        // Escapa el texto para HTML y conserva los saltos de línea
        private static string Html(string? texto) =>
            WebUtility.HtmlEncode(texto ?? "")
                .Replace("\r\n", "<br>")
                .Replace("\n", "<br>");

        // Una fila de la tabla de detalles
        private static string Fila(string etiqueta, string valor) => $@"
             <tr>
            <td style=""padding:4px 16px 4px 0;color:#555;white-space:nowrap;vertical-align:top;"">{etiqueta}</td>
            <td style=""padding:4px 0;"">{valor}</td>
            </tr>";

        public static string NuevaSistematizacion(Sistematizacion s)
        {
            // Notas: siempre se muestran
            var notas = string.IsNullOrWhiteSpace(s.Notas)
                ? "<em>Sin notas adicionales.</em>"
                : Html(s.Notas);

            // Material audiovisual: "persona: qué tiene"
            string material;
            var items = s.AudiosVideos?.Items;

            if (items is { Count: > 0 })
            {
                var lista = new StringBuilder("<ul style=\"margin:4px 0;padding-left:20px;\">");
                for (int i = 0; i < items.Count; i++)
                {
                    lista.Append($"<li><strong>{Html(items[i].Propietario)}</strong>: {Html(items[i].Titulo)}</li>");
                }
                lista.Append("</ul>");
                material = lista.ToString();
            }
            else
            {
                material = "<p style=\"margin:4px 0;\">No se registró material audiovisual.</p>";
            }

            // Detalles de la actividad
            var filas = new StringBuilder();
            filas.Append(Fila("Actividad", Html(s.Actividad)));
            filas.Append(Fila("Fecha", $"{s.Fecha:dd/MM/yyyy}"));
            filas.Append(Fila("Comunidad", Html(s.Comunidad)));
            filas.Append(Fila("Distrito", Html(s.Distrito)));
            filas.Append(Fila("Participantes", s.Participantes.ToString()));
            filas.Append(Fila("Horas", s.Horas.ToString()));

            return $@"
            <div style=""font-family:Arial,sans-serif;font-size:14px;color:#222;max-width:600px;"">
            <h2 style=""margin:0 0 12px;"">Nueva sistematización</h2>

                 <p><strong>{Html(s.Nombre)}</strong> te informa lo siguiente:</p>
                  <blockquote style=""margin:0 0 20px;padding:8px 16px;border-left:4px solid #0ea5e9;background:#f1f5f9;"">
                {notas}
                </blockquote>
    
                <h3 style=""margin:0 0 6px;font-size:15px;"">Detalles de la actividad</h3>
                 <table style=""border-collapse:collapse;margin-bottom:16px;"">{filas}</table>

                <h3 style=""margin:0 0 6px;font-size:15px;"">Material audiovisual</h3>
                {material}

                    <p style=""color:#888;font-size:12px;margin-top:24px;"">Mensaje automático del sistema TCU-782.</p>
                    </div>";
        }



    }




}