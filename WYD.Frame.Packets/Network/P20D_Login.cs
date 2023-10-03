using System.Runtime.InteropServices;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
[Packet(Id = 0x784)]
public struct P20D_Login : IPacket
{
    public NetworkHeader Header { get; set; }

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
    public string Senha;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
    public string Login;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 52)]
    public byte[] Unk1;

    public ushort Cliver;
    public ushort Unkown2;

    public int UmBool;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
    public string Hwid;

    public static P20D_Login Create(string login, string senha, string hwid, ushort cliver)
    {
        return new P20D_Login
        {
            Cliver = cliver,
            Senha =  senha,
            Login = login,
            Hwid = hwid,
            UmBool = 1
        };
    }
}