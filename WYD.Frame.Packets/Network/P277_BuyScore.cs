using System.Runtime.InteropServices;
using WYD.Frame.Common.Enum;

namespace WYD.Frame.Packets.Network;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Packet(Id = 0x277)]
public struct P277_BuyScore : IPacket
{
    public NetworkHeader Header { get; set; }
    //00 = compra de pontos, 01 compra de linhagem, 02 compra de skills
    public BonusType BonusType;
    public short Detail;
    public ushort NpcIndex;
    public short Unk;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bonusType"></param>
    /// <param name="detail">Em caso de compra de pontos é a offset, em caso de compra de skills, o Id da skill na itemlist.</param>
    /// <param name="npcIndex"></param>
    /// <returns></returns>
    public static P277_BuyScore Create(BonusType bonusType, short detail, ushort npcIndex)
    {
        return new P277_BuyScore()
        {
            BonusType = bonusType,
            Detail = detail,
            NpcIndex =  npcIndex
        };
    }
}