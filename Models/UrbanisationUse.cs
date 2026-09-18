namespace API_Backend_App_Honorarios.Models
{
    public class UrbanisationUse
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public double? GreenArea { get; set; }
        public double? NetworkArea { get; set; }
        public double? UnitPEMGreenArea { get; set;}
        public double? UnitPEMNetworkArea { get; set; }

        // METADATA
        public bool Valid { get; set; }
    }
}
