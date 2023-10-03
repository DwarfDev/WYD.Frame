using Newtonsoft.Json;

namespace WYD.Frame.Services.Models;

public class MoboInfo
{
    [JsonProperty("name")]
    public string Name;

    [JsonProperty("rating")]
    public int? Rating;

    [JsonProperty("rating_count")]
    public int? RatingCount;

    [JsonProperty("price_usd")]
    public double? PriceUsd;

    [JsonProperty("socket_/_cpu")]
    public string SocketCpu;

    [JsonProperty("form_factor")]
    public string FormFactor;

    [JsonProperty("memory_max")]
    public string MemoryMax;

    [JsonProperty("memory_slots")]
    public string MemorySlots;

    [JsonProperty("color")]
    public string Color;
}