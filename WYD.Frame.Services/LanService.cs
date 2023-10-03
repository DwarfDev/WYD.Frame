using System.Drawing;
using WYD.Frame.Common.Enum;
using WYD.Frame.Models.Models;
using WYD.Frame.Services.Models.Lan;

namespace WYD.Frame.Services;

public class LanService
{
    private static List<Lan>? _lans = null;
    private static List<Lan> Lans
    {
        get
        {
            if (_lans is null)
            {
                _lans = LoadLans();
            }

            return _lans;
        }
    }

    private static List<Lan> LoadLans()
    {
        var lans = new List<Lan>();
        
        //LanN
        
        lans.Add(new()
        {
            BlockSize = new Size { Height = 15, Width = 15 },
            LowerLimit = new Position
            {
                X = 3601,
                Y = 3601
            },
            UpperLimit = new Position
            {
                X = 3693,
                Y = 3693
            },
            LanSize = new Size
            {
                Height = 90,
                Width = 90
            },
            Type = LanType.LanN
        });

        return lans;
    }
  
    public static Lan GetLanByType(LanType lanType)
    {
        return Lans.First(x => x.Type == lanType);
    }
    
    
}