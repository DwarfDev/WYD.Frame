using WYD.Frame.Common;

namespace WYD.Frame.Models.Models;

public class ClientCredentials
{
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Numeric { get; set; } = "";
    public string Mac { get; set; } = Utils.GenerateHwid();
}