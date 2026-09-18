using System.Text.Json.Serialization;

namespace API_Backend_App_Honorarios.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(EdificationCalculationRequest), typeDiscriminator: "EDIF")]
    [JsonDerivedType(typeof(CivilWorksCalculationRequest), typeDiscriminator: "OBCI")]
    [JsonDerivedType(typeof(UrbanisationCalculationRequest), typeDiscriminator: "URBA")]

    public abstract class HonorariosCalculationRequest
    {
        public List<AddonInfo> SelectedAddons { get; set; } = new();
    }
}
