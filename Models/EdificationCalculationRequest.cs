namespace API_Backend_App_Honorarios.Models
{
    public class EdificationCalculationRequest : HonorariosCalculationRequest
    {
        public List<EdificationUse> Uses { get; set; } = new();
        public EdificationProjectState ProjectState { get; set; }
    }
}
