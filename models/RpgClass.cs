using System.Text.Json.Serialization;

namespace dotnetrpg.models
{
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum RpgClass
  {
    //   Cleric = Healer | Mage = wizard/witch
    Knight, Mage, Cleric
  }
}