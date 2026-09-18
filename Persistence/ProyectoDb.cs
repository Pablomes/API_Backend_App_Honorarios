using API_Backend_App_Industrializacion.Models;
using Microsoft.EntityFrameworkCore;

namespace API_Backend_App_Industrializacion.Persistence
{
    public class ProyectoDb : DbContext
    {
        public ProyectoDb(DbContextOptions<ProyectoDb> options) : base(options)
        {
        }

        public DbSet<Proyecto> Proyecto => Set<Proyecto>();
    }
}
