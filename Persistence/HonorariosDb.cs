using API_Backend_App_Honorarios.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace API_Backend_App_Honorarios.Persistence
{
    public class HonorariosDb : DbContext
    {
        private readonly string _schema;

        public HonorariosDb(DbContextOptions<HonorariosDb> options, IOptions<PersistenceSettings> settings) : base(options)
        {
            _schema = settings.Value.Schema ?? "Data";
        }

        public DbSet<EdificacionCalcData> EdificacionCalcDatas => Set<EdificacionCalcData>();
        public DbSet<EdificacionPlantasCalcData> EdificacionPlantasCalcDatas => Set<EdificacionPlantasCalcData>();
        public DbSet<EdificacionDocs> EdificacionDocs => Set<EdificacionDocs>();
        public DbSet<EdificacionTiposProyecto> EdificacionTiposProyecto => Set<EdificacionTiposProyecto>();

        public DbSet<ObraCivilCalcData> ObraCivilCalcDatas => Set<ObraCivilCalcData>();
        public DbSet<CoefHObraCivilCalcData> CoefHObraCivilCalcDatas => Set<CoefHObraCivilCalcData>();
        public DbSet<CoefsObraCivilCalcData> CoefsObraCivilCalcDatas => Set<CoefsObraCivilCalcData>();
        public DbSet<PEMRObraCivilCalcData> PEMRObraCivilCalcDatas => Set<PEMRObraCivilCalcData>();
        public DbSet<ObraCivilDocs> ObraCivilDocs => Set<ObraCivilDocs>();
        public DbSet<ObraCivilTiposProyecto> ObraCivilTiposProyecto => Set<ObraCivilTiposProyecto>();

        public DbSet<UrbanizacionCalcData> UrbanizacionCalcDatas => Set<UrbanizacionCalcData>();
        public DbSet<UrbanizacionDocs> UrbanizacionDocs => Set<UrbanizacionDocs>();
        public DbSet<UrbanizacionTiposProyecto> UrbanizacionTiposProyecto => Set<UrbanizacionTiposProyecto>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(_schema);

            modelBuilder.Entity<EdificacionCalcData>().ToTable("EdificacionCalcData");
            modelBuilder.Entity<EdificacionPlantasCalcData>().ToTable("EdificacionPlantasCalcData");
            modelBuilder.Entity<EdificacionDocs>(entity =>
            {
                entity.ToTable("EdificacionDocs");
                entity.Property(e => e.Info)
                      .IsRequired(false)
                      .HasConversion(
                          v => v == string.Empty ? null : v,
                          v => v ?? string.Empty
                      );
            });

            modelBuilder.Entity<EdificacionTiposProyecto>().ToTable("EdificacionTiposProyecto")
                .HasMany(p => p.Docs)
                .WithMany()
                .UsingEntity("EdificacionDocsProyecto",
                l => l.HasOne(typeof(EdificacionDocs)).WithMany().HasForeignKey("Doc"),
                r => r.HasOne(typeof(EdificacionTiposProyecto)).WithMany().HasForeignKey("Proy"));

            modelBuilder.Entity<ObraCivilCalcData>().ToTable("ObraCivilCalcData");
            modelBuilder.Entity<CoefHObraCivilCalcData>().ToTable("CoefHObraCivilCalcData");
            modelBuilder.Entity<CoefsObraCivilCalcData>().ToTable("CoefsObraCivilCalcData");
            modelBuilder.Entity<PEMRObraCivilCalcData>().ToTable("PEMRObraCivilCalcData");
            modelBuilder.Entity<ObraCivilDocs>(entity =>
            {
                entity.ToTable("ObraCivilDocs");
                entity.Property(e => e.Info)
                      .IsRequired(false)
                      .HasConversion(
                          v => v == string.Empty ? null : v,
                          v => v ?? string.Empty
                      );
            });
            modelBuilder.Entity<ObraCivilTiposProyecto>().ToTable("ObraCivilTiposProyecto")
                .HasMany(p => p.Docs)
                .WithMany()
                .UsingEntity("ObraCivilDocsProyecto",
                l => l.HasOne(typeof(ObraCivilDocs)).WithMany().HasForeignKey("Doc"),
                r => r.HasOne(typeof(ObraCivilTiposProyecto)).WithMany().HasForeignKey("Proy"));

            modelBuilder.Entity<UrbanizacionCalcData>().ToTable("UrbanizacionCalcData");
            modelBuilder.Entity<UrbanizacionDocs>(entity =>
            {
                entity.ToTable("UrbanizacionDocs");
                entity.Property(e => e.Info)
                      .IsRequired(false)
                      .HasConversion(
                          v => v == string.Empty ? null : v,
                          v => v ?? string.Empty
                      );
            });
            modelBuilder.Entity<UrbanizacionTiposProyecto>().ToTable("UrbanizacionTiposProyecto")
                .HasMany(p => p.Docs)
                .WithMany()
                .UsingEntity("UrbanizacionDocsProyecto",
                l => l.HasOne(typeof(UrbanizacionDocs)).WithMany().HasForeignKey("Doc"),
                r => r.HasOne(typeof(UrbanizacionTiposProyecto)).WithMany().HasForeignKey("Proy"));

            base.OnModelCreating(modelBuilder);
        }
    }
}
