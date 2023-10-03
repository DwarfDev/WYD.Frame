namespace WYD.Frame.Models.Models;

public class ProxyConfiguration
{
    public bool UseProxy { get; set; } = default;
    public string Host { get; set; } = "";
    public int Port { get; set; } = default;
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
}