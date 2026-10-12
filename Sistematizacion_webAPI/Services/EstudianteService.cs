using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Sistematizacion_webAPI.Settings;
using Sistematizacion_webAPI.Models;

namespace Sistematizacion_webAPI.Services
{
    public class EstudianteService
    {
        private readonly IMongoCollection<Estudiante> _estudiantes;

        // Recibe el cliente de Mongo ya creado (singleton) y solo obtiene
        // la base de datos y la colección "estudiantes"
        public EstudianteService(IMongoClient client, IOptions<MongoDbSettings> mongoDbSettings)
        {
            var database = client.GetDatabase(mongoDbSettings.Value.DatabaseName);
            _estudiantes = database.GetCollection<Estudiante>("estudiantes");
        }

        public async Task<List<Estudiante>> GetAllAsync() =>
            await _estudiantes.Find(_ => true).ToListAsync();

        public async Task<Estudiante?> GetByIdAsync(string id) =>
            await _estudiantes.Find(e => e.Id == id).FirstOrDefaultAsync();

        public async Task<List<Estudiante>> GetByIdsAsync(IEnumerable<string> ids)
        {
            var filter = Builders<Estudiante>.Filter.In(e => e.Id, ids);
            return await _estudiantes.Find(filter).ToListAsync();
        }

        public async Task CreateAsync(Estudiante nuevoEstudiante) =>
            await _estudiantes.InsertOneAsync(nuevoEstudiante);

        public async Task UpdateAsync(string id, Estudiante estudianteActualizado) =>
            await _estudiantes.ReplaceOneAsync(e => e.Id == id, estudianteActualizado);

        public async Task DeleteAsync(string id) =>
            await _estudiantes.DeleteOneAsync(e => e.Id == id);

        // Actualiza únicamente el campo isActive del estudiante indicado
        public async Task<bool> SetActiveStatusAsync(string id, bool isActive)
        {
            var update = Builders<Estudiante>.Update.Set(e => e.IsActive, isActive);
            var result = await _estudiantes.UpdateOneAsync(e => e.Id == id, update);
            return result.ModifiedCount > 0;
        }
    }
}