using API_Backend_App_Honorarios.Models;
using API_Backend_App_Industrializacion.Models;
using API_Backend_App_Industrializacion.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API_Backend_App_Honorarios.Persistence
{
    public class EdificacionPlantasCalcDataDb : DbContext
    {
        public EdificacionPlantasCalcDataDb(DbContextOptions<EdificacionPlantasCalcDataDb> options) : base(options)
        {
        }

        public DbSet<EdificacionPlantasCalcData> EdificacionPlantasCalcDatas => Set<EdificacionPlantasCalcData>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EdificacionPlantasCalcData>().ToTable("EdificacionPlantasCalcData", schema: "Data");

            base.OnModelCreating(modelBuilder);
        }
    }
}
