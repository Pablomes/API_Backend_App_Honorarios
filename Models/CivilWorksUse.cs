namespace API_Backend_App_Honorarios.Models
{
    public class CivilWorksUse
    {
        public int Id { get; set; }
        public string Type { get; set; } = "";
        public double? Area { get; set; }
        public double? UnitPEM { get; set; }
        public double? TotalPEM { get; set; }

        // METADATA
        public bool TotalPEMManual { get; set; }

        public bool Valid { get; set; }
    }
}
