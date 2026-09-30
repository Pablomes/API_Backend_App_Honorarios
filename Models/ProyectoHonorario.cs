using System.ComponentModel.DataAnnotations;

namespace API_Backend_App_Honorarios.Models
{
    public class ProyectoHonorario
    {
        [Key]
        public string CVE { get; set; }

        [Required]
        public string ProjectType { get; set; }

        [Required]
        public string ActuationId { get; set; }

        [Required]
        public DateTime FechaHoraCreacion { get; set; }

    }
}
