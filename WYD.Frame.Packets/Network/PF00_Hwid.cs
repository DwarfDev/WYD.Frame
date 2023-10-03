using System.Buffers.Text;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using WYD.Frame.Common;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0xF00)]
public struct PF00_Hwid : IPacket
{
    public NetworkHeader Header { get; set; }

    public uint TresUnk; // 03 00 00 00 


    public long Unkw1; //D8 5E D3 F3 99 5F 00 00 
    public byte Unkw2; //00

    public long Unk3; // 15 5D 2C 18 4B 00 00 00 

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
    public byte[] Unk4; //00 00 00 00 00 00 00 

    public int Unk5; //02 00 00 00 



    public static  PF00_Hwid Create()
    {
        return new PF00_Hwid()
        {
        };
    }
    
}