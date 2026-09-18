using System.ComponentModel.DataAnnotations;

namespace API_Backend_App_Honorarios.Models
{
    public class PEMRObraCivilCalcData
    {
        [Key]
        public int Presupuesto { get; set; }

        [Required]
        public int Reducido { get; set; }

        [Required]
        public double Coef { get; set; }
    }
}
