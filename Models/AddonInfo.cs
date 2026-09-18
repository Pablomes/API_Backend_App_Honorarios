namespace API_Backend_App_Honorarios.Models
{
    public class AddonInfo
    {
        public string Id { get; set; } = "";
        public List<bool> EnabledUses { get; set; } = new();
    }
}
