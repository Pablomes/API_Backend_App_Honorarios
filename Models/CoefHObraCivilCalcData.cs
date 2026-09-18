using System.ComponentModel.DataAnnotations;

namespace API_Backend_App_Honorarios.Models
{
    public class CoefHObraCivilCalcData
    {
        [Key]
        public string ID { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public double Coef { get; set; }
    }
}
