using System.ComponentModel.DataAnnotations;

namespace API_Backend_App_Honorarios.Models
{
    public class CoefsObraCivilCalcData
    {
        [Key]
        public string Id { get; set; }

        public string? Nombre { get; set; }

        [Required]
        public double Coef { get; set; }
    }
}
