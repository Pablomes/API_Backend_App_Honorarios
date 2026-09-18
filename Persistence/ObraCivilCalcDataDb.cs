using API_Backend_App_Honorarios.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Backend_App_Honorarios.Persistence
{
    public class ObraCivilCalcDataDb : DbContext
    {
        public ObraCivilCalcDataDb(DbContextOptions<ObraCivilCalcDataDb> options) : base(options)
        {
        }

        public DbSet<ObraCivilCalcData> ObraCivilCalcDatas => Set<ObraCivilCalcData>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ObraCivilCalcData>().ToTable("ObraCivilCalcData", schema: "Data");

            base.OnModelCreating(modelBuilder);
        }
    }
}
