using System.ComponentModel.DataAnnotations;

namespace API_Backend_App_Honorarios.Models
{
    public abstract class GenericProjectType<TDoc> where TDoc : GenericDoc
    {
        [Key]
        public string ID { get; set; }
        [Required]
        public string Nombre { get; set; }

        public List<TDoc> Docs { get; set; } = new();

        [Required]
        public int OrderIdx { get; set; }
    }
}
