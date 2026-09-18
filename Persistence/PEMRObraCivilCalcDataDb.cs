using API_Backend_App_Honorarios.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Backend_App_Honorarios.Persistence
{
    public class PEMRObraCivilCalcDataDb : DbContext
    {
        public PEMRObraCivilCalcDataDb(DbContextOptions<PEMRObraCivilCalcDataDb> options) : base(options)
        {
        }

        public DbSet<PEMRObraCivilCalcData> PEMRObraCivilCalcDatas => Set<PEMRObraCivilCalcData>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PEMRObraCivilCalcData>().ToTable("PEMRObraCivilCalcData", schema: "Data");

            base.OnModelCreating(modelBuilder);
        }
    }
}
