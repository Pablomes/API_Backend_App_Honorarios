using API_Backend_App_Industrializacion.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using API_Backend_App_Honorarios.Persistence;

namespace API_Backend_App_Industrializacion.Persistence
{
    public class ProyectoDb : DbContext
    {
        private readonly string _schema;

        public ProyectoDb(DbContextOptions<ProyectoDb> options, IOptions<PersistenceSettings> settings) : base(options)
        {
            _schema = settings.Value.Schema ?? "Data";
        }

        public DbSet<Proyecto> Proyecto => Set<Proyecto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(_schema);

            base.OnModelCreating(modelBuilder);
        }
    }
}
