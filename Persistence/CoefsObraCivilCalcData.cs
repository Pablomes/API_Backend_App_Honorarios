using API_Backend_App_Honorarios.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Backend_App_Honorarios.Persistence
{
    public class CoefsObraCivilCalcDataDb : DbContext
    {
        public CoefsObraCivilCalcDataDb(DbContextOptions<CoefsObraCivilCalcDataDb> options) : base(options)
        {
        }

        public DbSet<CoefsObraCivilCalcData> CoefsObraCivilCalcDatas => Set<CoefsObraCivilCalcData>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CoefsObraCivilCalcData>().ToTable("CoefsObraCivilCalcData", schema: "Data");

            base.OnModelCreating(modelBuilder);
        }
    }
}