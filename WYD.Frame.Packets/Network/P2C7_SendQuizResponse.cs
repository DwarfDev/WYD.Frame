using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x2C7)]
public struct P2C7_SendQuizResponse : IPacket
{
    public NetworkHeader Header { get; set; }
    public int Response;
    public int UmBool;

    public static P2C7_SendQuizResponse Create(int response)
    {
        return new P2C7_SendQuizResponse
        {
            Response = response,
            UmBool =  1
        };
    }
}