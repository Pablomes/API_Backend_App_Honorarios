using System.ComponentModel.DataAnnotations;

namespace API_Backend_App_Industrializacion.Models
{
    public class Proyecto
    {
        [Key]
        public string CVE { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Ubicacion { get; set; }

        [Required]
        public string Promotor { get; set; }

        [Required]
        public string Proyectista { get; set; }

        [Required]
        public DateTime FechaHoraCreacion { get; set; }

    }
}
