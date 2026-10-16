using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Sistematizacion_webAPI.Settings;
using Sistematizacion_webAPI.Models;

namespace Sistematizacion_webAPI.Services
{
    public class ConfigEmailService
    {
        private readonly IMongoCollection<ConfigEmail> _configEmail;

        public ConfigEmailService(IMongoClient client, IOptions<MongoDbSettings> mongoDbSettings)
        {
            var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _configEmail = database.GetCollection<ConfigEmail>("configEmail");
        }

        // Si el documento no existe, devuelve valores seguros (no enviar)
        public async Task<ConfigEmail> GetAsync() =>
            await _configEmail.Find(c => c.Id == "correo").FirstOrDefaultAsync()
            ?? new ConfigEmail();

        // Crea o reemplaza el documento único de configuración
        public async Task SaveAsync(ConfigEmail config)
        {
            config.Id = "correo";
            await _configEmail.ReplaceOneAsync(
                c => c.Id == "correo", config, new ReplaceOptions { IsUpsert = true });
        }

        // Cambia únicamente enviarCorreos; el resto del documento no se toca
        public async Task SetSendEmailsAsync(bool valor)
        {
            var update = Builders<ConfigEmail>.Update.Set(c => c.EnviarCorreos, valor);
            await _configEmail.UpdateOneAsync(
                c => c.Id == "correo", update, new UpdateOptions { IsUpsert = true });
        }
    }
}