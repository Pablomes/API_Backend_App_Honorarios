using API_Backend_App_Honorarios.Models;
using API_Backend_App_Industrializacion.Models;
using API_Backend_App_Industrializacion.Persistence;
using Microsoft.EntityFrameworkCore;

namespace API_Backend_App_Honorarios.Persistence
{
    public class EdificacionCalcDataDb : DbContext
    {
        public EdificacionCalcDataDb(DbContextOptions<EdificacionCalcDataDb> options) : base(options)
        {
        }

        public DbSet<EdificacionCalcData> EdificacionCalcDatas => Set<EdificacionCalcData>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EdificacionCalcData>().ToTable("EdificacionCalcData", schema: "Data");

            base.OnModelCreating(modelBuilder);
        }
    }
}
