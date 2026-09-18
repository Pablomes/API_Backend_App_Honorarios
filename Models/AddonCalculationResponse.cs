namespace API_Backend_App_Honorarios.Models
{
    public class AddonCalculationResponse
    {
        public string Id { get; set; } = "";
        public List<double> Values { get; set; } = new();

        public AddonCalculationResponse() { }

        public AddonCalculationResponse(string Id)
        {
            this.Id = Id;
        }
    }
}
