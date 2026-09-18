namespace API_Backend_App_Honorarios.Models
{
    public class EdificationUse
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int? RepeatedFloors { get; set; }
        public double? Area { get; set; }
        public double? UnitPEM { get; set; }
        public double? InstallationPEM { get; set; }

        //METADATA
        public bool Valid { get; set; }
    }
}
