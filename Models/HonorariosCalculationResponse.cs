namespace API_Backend_App_Honorarios.Models
{
    public class HonorariosCalculationResponse
    {
        public List<double> ProjectCosts { get; set; } = new();
        public List<AddonCalculationResponse> Responses { get; set; } = new();
    }
}
