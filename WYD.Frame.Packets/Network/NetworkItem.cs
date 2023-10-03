using System.Runtime.InteropServices;
using WYD.Frame.Common.Enum.Game;

namespace WYD.Frame.Packets.Network;

public struct NetworkItem
{
    public short Id;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public NetworkItemEf[] Ef;

    public static NetworkItem Create()
    {
        return new NetworkItem()
        {
            Ef = new NetworkItemEf[3]
        };
    }

    public int GetAmount()
    {
        var amountEf = Ef.FirstOrDefault(x => x.Type == GameEfv.EF_AMOUNT);
        return amountEf.Value == 0 ? 1 : amountEf.Value;
    }

    public int GetRefinationLevel()
    {
        var amountEf = Ef.FirstOrDefault(x => x.Type == GameEfv.EF_SANC);

        return amountEf.Value;
    }
}