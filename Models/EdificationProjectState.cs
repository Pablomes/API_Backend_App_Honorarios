using System.Text.Json.Serialization;

namespace API_Backend_App_Honorarios.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EdificationProjectState
    {
        ESPR,
        ANPR,
        PRBA,
        PREJ,
        PBEJ,
        DIOB,
        PBED,
        ATSU,
        ACPR
    }
}