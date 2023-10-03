using WYD.Frame.Models.Models;
using WYD.Frame.Services.Common;

namespace WYD.Frame.Services.Models.Location;

public class Portal
{
    public Position Location { get; set; }
    public AreaNames LeadsTo { get; set; }
}