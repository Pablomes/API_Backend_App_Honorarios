using API_Backend_App_Honorarios.ExpressionInterpreting;
using API_Backend_App_Honorarios.Models;
using API_Backend_App_Honorarios.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace API_Backend_App_Honorarios.Services
{
    public class FetchService
    {
        protected readonly HonorariosDb honorariosDb;

        public FetchService(HonorariosDb honorariosDb)
        {
            this.honorariosDb = honorariosDb;
        }

        public async Task<List<EdificacionDocs>> GetEdificationDocs()
        {
            return await this.honorariosDb.EdificacionDocs.ToListAsync();
        }

        public async Task<List<ObraCivilDocs>> GetCivilWorksDocs()
        {
            return await this.honorariosDb.ObraCivilDocs.ToListAsync();
        }

        public async Task<List<UrbanizacionDocs>> GetUrbanisationDocs()
        {
            return await this.honorariosDb.UrbanizacionDocs.ToListAsync();
        }

        public async Task<List<EdificacionTiposProyecto>> GetEdificationProjects()
        {
            return await this.honorariosDb.EdificacionTiposProyecto.Include(p => p.Docs.OrderBy(d => d.OrderIdx)).OrderBy(p => p.OrderIdx).ToListAsync();
        }

        public async Task<List<ObraCivilTiposProyecto>> GetCivilWorksProjects()
        {
            return await this.honorariosDb.ObraCivilTiposProyecto.Include(p => p.Docs.OrderBy(d => d.OrderIdx)).OrderBy(p => p.OrderIdx).ToListAsync();
        }

        public async Task<List<UrbanizacionTiposProyecto>> GetUrbanisationProjects()
        {
            return await this.honorariosDb.UrbanizacionTiposProyecto.Include(p => p.Docs.OrderBy(d => d.OrderIdx)).OrderBy(p => p.OrderIdx).ToListAsync();
        }

        public async Task<EdificacionDocs> GetEdificationDocById(string Id)
        {
            return await this.honorariosDb.EdificacionDocs.FindAsync(Id);
        }

        public async Task<ObraCivilDocs> GetCivilWorksDocById(string Id)
        {
            return await this.honorariosDb.ObraCivilDocs.FindAsync(Id);
        }

        public async Task<UrbanizacionDocs> GetUrbanisationDocById(string Id)
        {
            return await this.honorariosDb.UrbanizacionDocs.FindAsync(Id);
        }

        public async Task<EdificacionTiposProyecto> GetEdificationProjectById(string Id)
        {
            return await this.honorariosDb.EdificacionTiposProyecto.FindAsync(Id);
        }

        public async Task<ObraCivilTiposProyecto> GetCivilWorksProjectById(string Id)
        {
            return await this.honorariosDb.ObraCivilTiposProyecto.FindAsync(Id);
        }

        public async Task<UrbanizacionTiposProyecto> GetUrbanisationProjectById(string Id)
        {
            return await this.honorariosDb.UrbanizacionTiposProyecto.FindAsync(Id);
        }
    }
}