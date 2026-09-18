using System.Text.Json.Serialization;

namespace API_Backend_App_Honorarios.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UrbanisationProjectState
    {
        ANPR,
        PROY,
        DIOB,
        EIAP,
        EIPP
    }
}