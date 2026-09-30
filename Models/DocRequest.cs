using API_Backend_App_Honorarios.Models;

namespace API_Backend_App_Honorarios.Models
{
    public class DocRequest
    {
        public string ProjectType { get; set; }
        public string ActuationId { get; set; }
        public HonorariosCalculationRequest CalculationRequest { get; set; }

        /*

        public string ProjectName { get; set; }
        public string Location { get; set; }
        public string Developer { get; set; }
        public string Projector { get; set; }

        public double TotalHonorarios { get; set; }
        public double CompPrefabricados { get; set; }
        public double ReduccionTiempo { get; set; }
        public List<SectionDocRequest> Sections { get; set; } = [];

        public DocRequest(double TotalHonorarios, double CompPrefabricados, double ReduccionTiempo, string ProjectName, string Location, string Developer, string Projector, List<SectionDocRequest> Sections)
        {
            this.TotalHonorarios = TotalHonorarios;
            this.CompPrefabricados = CompPrefabricados;
            this.ReduccionTiempo = ReduccionTiempo;

            this.ProjectName = ProjectName;
            this.Location = Location;
            this.Developer = Developer;
            this.Projector = Projector;

            this.Sections = Sections;
        }

        public override string ToString()
        {
            string globalString = "";

            foreach (SectionDocRequest section in Sections)
            {
                globalString += $"{section.ToString()}, ";
            }

            return $"DocRequest : {{ TotalHonorarios : {TotalHonorarios}, CompPrefabricados : {CompPrefabricados}, ReduccionTiempo : {ReduccionTiempo}, ProjectName : {ProjectName}, Location: {Location}, Developer : {Developer}, Projector : {Projector}, Sections : {globalString} }}";
        }

        */
    }
}
