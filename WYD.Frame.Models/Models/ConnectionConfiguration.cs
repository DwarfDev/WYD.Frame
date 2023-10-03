namespace WYD.Frame.Models.Models;

public class ConnectionConfiguration
{
    public string ServerIp { get; set; } = "";
    public int ServerPort { get; set; } = default;
    public ushort ServerClientVersion { get; set; } = default;
}