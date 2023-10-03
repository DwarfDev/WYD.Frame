using Newtonsoft.Json;

namespace WYD.Frame.Services.Models;

public class HdInfo
{
    [JsonProperty("name")]
    public string Name;

    [JsonProperty("rating")]
    public int? Rating;

    [JsonProperty("rating_count")]
    public int? RatingCount;

    [JsonProperty("price_usd")]
    public double? PriceUsd;

    [JsonProperty("capacity")]
    public string Capacity;

    [JsonProperty("price_/_gb")]
    public string PriceGb;

    [JsonProperty("type")]
    public string Type;

    [JsonProperty("cache")]
    public string Cache;

    [JsonProperty("form_factor")]
    public string FormFactor;

    [JsonProperty("interface")]
    public string Interface;
}