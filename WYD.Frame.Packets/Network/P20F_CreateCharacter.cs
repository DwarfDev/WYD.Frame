using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
[Packet(Id = 0x20F)]
public struct P20F_CreateCharacter : IPacket
{
    public NetworkHeader Header { get; set; }
    public int CharIndex;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
    public string Nickname;
    public int Unkw;

    public static P20F_CreateCharacter Create(int charIndex, string nickname)
    {
        var packet = new P20F_CreateCharacter
        {
            CharIndex = charIndex, Nickname = nickname, Unkw = 1
        };


        return packet;
    }
}