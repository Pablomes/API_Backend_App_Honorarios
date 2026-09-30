using API_Backend_App_Honorarios.Persistence;
using API_Backend_App_Honorarios.Util;
using API_Backend_App_Honorarios.Models;

namespace API_Backend_App_Honorarios.Services
{
    public class ProjectPersistenceService
    {
        private readonly HonorariosDb _db;
        private readonly Base58 _base58;

        public ProjectPersistenceService(HonorariosDb db)
        {
            _db = db;
            _base58 = new Base58();
        }

        public async Task<bool> checkProjectExists(string cveTag)
        {
            ProyectoHonorario? proyecto = await this._db.Proyectos.FindAsync(cveTag);

            return proyecto == null ? false : true;
        }

        public async Task<string> generateRandomCVECode()
        {
            string code = "";

            while (await checkProjectExists(code = $"NI{this._base58.randomCode(19)}")) { }

            return code;
        }

        public async Task<ProyectoHonorario> RegisterProjectAsync(string projectType, string actuationId)
        {
            // Asume que la implementación Base58 tiene un método para generar el UUID
            string newUuid = await generateRandomCVECode();

            var project = new ProyectoHonorario()
            {
                CVE = newUuid,
                ProjectType = projectType,
                ActuationId = actuationId,
                FechaHoraCreacion = DateTime.Now
            };

            _db.Proyectos.Add(project);
            await _db.SaveChangesAsync();

            return project;
        }
    }
}