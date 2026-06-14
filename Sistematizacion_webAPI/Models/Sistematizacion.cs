using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sistematizacion_webAPI.Models
{
    public class Sistematizacion 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [BsonElement("comunidad")]
        public string Comunidad { get; set; } = string.Empty;

        [BsonElement("fecha")]
        public DateTime Fecha { get; set; }

        [BsonElement("actividad")]
        public string Actividad { get; set; } = string.Empty;

        [BsonElement("horas")]
        public int Horas { get; set; }

        [BsonElement("participantes")]
        public int Participantes { get; set; }

        [BsonElement("audiosVideos")]
        public AudiosVideos AudiosVideos { get; set; } = new();

        [BsonElement("consentimientos")]
        public Consentimientos Consentimientos { get; set; } = new();

        [BsonElement("notas")]
        public string Notas { get; set; } = string.Empty;

        [BsonElement("poblacion")]
        public List<string> Poblacion { get; set; } = new();

        [BsonElement("distrito")]
        public string Distrito { get; set; } = string.Empty;
    }

    public class AudiosVideos
    {
        [BsonElement("recopilado")]
        public bool Recopilado { get; set; }

        [BsonElement("items")]
        public List<AudioVideoItem> Items { get; set; } = new();
    }

    public class AudioVideoItem
    {
        [BsonElement("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [BsonElement("propietario")]
        public string Propietario { get; set; } = string.Empty;
    }

    public class Consentimientos
    {
        [BsonElement("tiene")]
        public bool Tiene { get; set; }

        [BsonElement("personas")]
        public List<PersonaConsentimiento> Personas { get; set; } = new();
    }

    public class PersonaConsentimiento
    {
        [BsonElement("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [BsonElement("cantidad")]
        public int Cantidad { get; set; }
    }
}