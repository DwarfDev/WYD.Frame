namespace WYD.Frame.Models.Models;

public class ClientConfiguration
{
    public ProxyConfiguration ProxyConfiguration { get; set; } = new();
    public ConnectionConfiguration ConnectionConfiguration { get; set; } = new();
    public ClientCredentials Credentials { get; set; } = new();
    public HwidInfo HwidInfo { get; set; } = new();
    public QuizConfiguration QuizConfiguration { get; set; } = new();
    public GeneralConfig GeneralConfig { get; set; } = new();
}