using MongoDB.Bson.Serialization.Attributes;

namespace Sistematizacion_webAPI.Models
{
    public class ConfigEmail
    {
        [BsonId]
        public string Id { get; set; } = "correo";

        [BsonElement("enviarCorreos")]
        public bool EnviarCorreos { get; set; } = false;

        [BsonElement("correoProfesora")]
        public string CorreoProfesora { get; set; } = "";

        [BsonElement("linkDrive")]
        public string LinkDrive { get; set; } = "";
    }
}