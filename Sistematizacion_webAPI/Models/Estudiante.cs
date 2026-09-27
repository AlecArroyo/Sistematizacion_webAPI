using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sistematizacion_webAPI.Models
{
    public class Estudiante
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault]
        public string? Id { get; set; }

        [BsonElement("nombre")]
        public string Nombre { get; set; } = null!;

        [BsonElement("apellido1")]
        public string Apellido1 { get; set; } = null!;

        [BsonElement("apellido2")]
        public string Apellido2 { get; set; } = null!;

        [BsonElement("email")]
        public string Email { get; set; } = null!;

        [BsonElement("isActive")]
        public bool IsActive { get; set; } = true;
    }
}