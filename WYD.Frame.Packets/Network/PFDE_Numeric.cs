using System.Runtime.InteropServices;
using System.Text;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0xFDE)]
public struct PFDE_Numeric : IPacket
{
    public NetworkHeader Header { get; set; }

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    public byte[] Numeric;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 14)]
    public byte[] Unk;

    public static PFDE_Numeric Create(string numerica)
    {
        var packet = new PFDE_Numeric
        {
            Numeric = FixNumericDigits(numerica)
        };
        return packet;
    }

    private static byte[] FixNumericDigits(string numerica)
    {
        var numericBytes = Encoding.UTF8.GetBytes(numerica);
        if (numericBytes.Length == 6) return numericBytes;
        return Enumerable.Range(0, 6).Select(x => x < numericBytes.Length ? numericBytes[x] : (byte)0).ToArray();
    }
}