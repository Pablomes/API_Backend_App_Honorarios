using API_Backend_App_Honorarios.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Backend_App_Honorarios.Persistence
{
    public class UrbanizacionCalcDataDb : DbContext
    {
        public UrbanizacionCalcDataDb(DbContextOptions<UrbanizacionCalcDataDb> options) : base(options)
        {
        }

        public DbSet<UrbanizacionCalcData> UrbanizacionCalcDatas => Set<UrbanizacionCalcData>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UrbanizacionCalcData>().ToTable("UrbanizacionCalcData", schema: "Data");

            base.OnModelCreating(modelBuilder);
        }
    }
}
