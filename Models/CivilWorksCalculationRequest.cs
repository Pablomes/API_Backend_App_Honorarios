namespace API_Backend_App_Honorarios.Models
{
    public class CivilWorksCalculationRequest : HonorariosCalculationRequest
    {
        public List<CivilWorksUse> Uses { get; set; } = new();
        public CivilWorksProjectState ProjectState { get; set; }
    }
}
