using System.ComponentModel.DataAnnotations;

namespace API_Backend_App_Honorarios.Models
{
    public abstract class GenericDoc
    {
        [Key]
        public string ID { get; set; }
        [Required]
        public string Nombre { get; set; }
        public string Info { get; set; }
        [Required]
        public bool ToggleUseCase { get; set; } = false;

        [Required]
        public int OrderIdx { get; set; }
    }
}
