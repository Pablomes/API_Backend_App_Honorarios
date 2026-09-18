namespace API_Backend_App_Honorarios.Models
{
    public class UrbanisationCalculationRequest : HonorariosCalculationRequest
    {
        public List<UrbanisationUse> Uses { get; set; } = new();
        public UrbanisationProjectState ProjectState { get; set; }
    }
}
