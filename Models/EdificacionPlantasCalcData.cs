using System.ComponentModel.DataAnnotations;

namespace API_Backend_App_Honorarios.Models
{
    public class EdificacionPlantasCalcData
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public double Coef { get; set; }
    }
}
