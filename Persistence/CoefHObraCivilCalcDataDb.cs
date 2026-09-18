using API_Backend_App_Honorarios.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Backend_App_Honorarios.Persistence
{
    public class CoefHObraCivilCalcDataDb : DbContext
    {
        public CoefHObraCivilCalcDataDb(DbContextOptions<CoefHObraCivilCalcDataDb> options) : base(options)
        {
        }

        public DbSet<CoefHObraCivilCalcData> CoefHObraCivilCalcDatas => Set<CoefHObraCivilCalcData>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CoefHObraCivilCalcData>().ToTable("CoefHObraCivilCalcData", schema: "Data");

            base.OnModelCreating(modelBuilder);
        }
    }
}