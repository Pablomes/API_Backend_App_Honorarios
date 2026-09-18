namespace API_Backend_App_Industrializacion.Models
{
    public class DocParameters
    {

        /*
        public string ProjectName { get; set; }
        public string Location { get; set; }
        public string Developer { get; set; }
        public string Projector { get; set; }


        public double TotalIndustrializacion { get; set; }
        public double CompPrefabricados { get; set; }
        public double ReduccionTiempo { get; set; }
        public Dictionary<string, double> SectionCompPrefabricados { get; set; }

        public Dictionary<string, double> SectionReduccionTiempo { get; set; }

        public Dictionary<string, Dictionary<string, string>> SubsectionLabelValues { get; set; }

        public DocParameters()
        {
            SectionCompPrefabricados = new Dictionary<string, double>();
            SectionReduccionTiempo = new Dictionary<string, double>();
            SubsectionLabelValues = new Dictionary<string, Dictionary<string, string>>();
        }

        public DocParameters(double TotalIndustrializacion, double CompPrefabricados, double ReduccionTiempo, string ProjectName, string Location, string Developer, string Projector) : this()
        {
            this.TotalIndustrializacion = TotalIndustrializacion;
            this.CompPrefabricados = CompPrefabricados;
            this.ReduccionTiempo = ReduccionTiempo;

            this.Location = Location;
            this.Projector = Projector;
            this.Developer = Developer;
            this.ProjectName = ProjectName;
        }

        public DocParameters(DocRequest request) : this()
        {
            /*

            this.TotalIndustrializacion = request.TotalIndustrializacion;
            this.CompPrefabricados = request.CompPrefabricados;
            this.ReduccionTiempo = request.ReduccionTiempo;

            this.ProjectName = request.ProjectName;
            this.Location = request.Location;
            this.Developer = request.Developer;
            this.Projector = request.Projector;

            foreach (SectionDocRequest section in request.Sections)
            {
                this.SectionCompPrefabricados.Add(section.ID, section.CompPrefabricados);
                this.SectionReduccionTiempo.Add(section.ID, section.ReduccionTiempo);

                this.SubsectionLabelValues.Add(section.ID, new Dictionary<string, string>());

                foreach (SubsectionDocRequest subsection in section.Subsections)
                {
                    this.SubsectionLabelValues[section.ID].Add(subsection.ID, subsection.Label);
                }
            }
        }
            */

    }
}
