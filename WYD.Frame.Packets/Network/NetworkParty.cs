using System.Runtime.InteropServices;
using WYD.Frame.Common.Enum.Game;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct NetworkParty
{
    public MobClass MobClass;
    public byte PartyIndex;
    public short Level;
    public short CurHp;
    public short MaxHp;
    public ushort ClientId;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
    public string Name;

    public static NetworkParty Create(ushort requesterId, string leaderName, byte partyIndex, short level,
        short currentHp, short maxHp, MobClass currentClass)
    {
        return new NetworkParty()
        {
            ClientId = requesterId,
            Name = leaderName,
            PartyIndex = partyIndex,
            CurHp = currentHp,
            MaxHp = maxHp,
            Level = level,
            MobClass = currentClass
        };
    }
}