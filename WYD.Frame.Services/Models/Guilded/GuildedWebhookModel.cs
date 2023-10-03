using Newtonsoft.Json;

namespace WYD.Frame.Services.Models.Guilded;

public class Embed
{
    [JsonProperty("description")] public string Description;

    [JsonProperty("footer")] public Footer Footer;

    [JsonProperty("title")] public string Title;
}

public class Footer
{
    [JsonProperty("text")] public string Text;
}

public class GuildedWebhookModel
{
    [JsonProperty("embeds")] public List<Embed> Embeds;
}