using System.Runtime.InteropServices;
using WYD.Frame.Common.Enum;

namespace WYD.Frame.Services.Models.Resource;

public class ItemInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ItemType Type { get; set; }
    public ItemClass ItemClass { get; set; }
    public TItemList ItemListItem { get; set; }
}

public struct TItemList
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
    public byte[] Name;
    public short nIndexMesh;
    public short nIndexTexture;
    public short nIndexVisualEffect;
    public short nReqLvl;
    public short nReqStr;
    public short nReqInt;
    public short nReqDex;
    public short nReqCon;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 12)]
    public TStaticEffect[] stEffect;
    public long nPrice;
    public short nUnique;
    public short nPos;
    public short nExtra;
    public short nGrade;
    
    public ItemClass GetItemClass()
    {
        var ef = stEffect.FirstOrDefault(x => x.sEffect == 87);
        return (ItemClass)ef.sValue;
    }
}

public struct TStaticEffect
{
    public short sEffect;
    public short sValue;
};