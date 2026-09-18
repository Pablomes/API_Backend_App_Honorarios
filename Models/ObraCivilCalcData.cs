using System.ComponentModel.DataAnnotations;

namespace API_Backend_App_Honorarios.Models
{
    public class ObraCivilCalcData
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Formula { get; set; }
    }
}
