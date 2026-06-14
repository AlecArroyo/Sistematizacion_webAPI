using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Sistematizacion_webAPI.Models;
using Sistematizacion_webAPI.Settings;

namespace Sistematizacion_webAPI.Services
{
    public class SistematizacionService
    {
        private readonly IMongoCollection<Sistematizacion> _collection;

        public SistematizacionService(
            IMongoClient mongoClient,
            IOptions<MongoDbSettings> settings)
        {
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<Sistematizacion>(settings.Value.CollectionName);
        }

        // Obtener todos
        public async Task<List<Sistematizacion>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        // Obtener por ID
        public async Task<Sistematizacion?> GetByIdAsync(string id) =>
            await _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

        // Crear
        public async Task CreateAsync(Sistematizacion sistematizacion) =>
            await _collection.InsertOneAsync(sistematizacion);

        // Actualizar
        public async Task UpdateAsync(string id, Sistematizacion sistematizacion) =>
            await _collection.ReplaceOneAsync(x => x.Id == id, sistematizacion);

        // Eliminar
        public async Task DeleteAsync(string id) =>
            await _collection.DeleteOneAsync(x => x.Id == id);
    }
}