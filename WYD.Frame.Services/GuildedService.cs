using System.Text;
using Newtonsoft.Json;
using WYD.Frame.Common.Enum;
using WYD.Frame.Services.Models.Guilded;

namespace WYD.Frame.Services;

/// <summary>
/// Serviço responsável por enviar notificações para o Guilded(é uma alternativa do Discord)
/// </summary>
public static class GuildedService
{
    private static readonly string LogChannel =
        "";

    private static readonly string WhisperChannel =
        "";

    private static readonly string DeathChannel =
        "";


    public static void Log(GuildedLogType type, string message, string title, string footer)
    {
        using var client = new HttpClient();
        var data = new GuildedWebhookModel
        {
            Embeds = new List<Embed>
            {
                new()
                {
                    Description = message,
                    Title = title,
                    Footer = new Footer
                    {
                        Text = footer
                    }
                }
            }
        };
        var json = JsonConvert.SerializeObject(data);
        var contentR = new StringContent(json, Encoding.UTF8, "application/json");
        string url;

        switch (type)
        {
            case GuildedLogType.Behavior:
                url = LogChannel;
                break;
            case GuildedLogType.Death:
                url = DeathChannel;
                break;
            case GuildedLogType.Whisper:
                url = WhisperChannel;
                break;
            default:
                url = LogChannel;
                break;
        }

        var response = client.PostAsync(
            url,
            contentR).Result;
    }
}