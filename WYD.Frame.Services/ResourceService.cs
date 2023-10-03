using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using WYD.Frame.Common;
using WYD.Frame.Common.Enum;
using WYD.Frame.Common.Enum.Game;
using WYD.Frame.Services.Models;
using WYD.Frame.Services.Models.Resource;

namespace WYD.Frame.Services;

public static class ResourceService
{
    private static readonly List<SkillInfo> _skills = new();
    private static readonly List<ItemInfo> _items = new();
    private static readonly List<HdInfo> _hds = new();
    private static readonly List<MoboInfo> _mobos = new();

    private static readonly Image<Rgba32> HeightMapImage =
        Image.Load<Rgba32>(Environment.CurrentDirectory + "/Resource/HeightMap.jpg");

    public static List<MoboInfo> Mobos
    {
        get
        {
            if (_mobos.Count == 0) SetMobos();

            return _mobos;
        }
    }

    public static List<HdInfo> Hds
    {
        get
        {
            if (_hds.Count == 0) SetHds();

            return _hds;
        }
    }

    public static List<ItemInfo> Items
    {
        get
        {
            if (_items.Count == 0) SetItems();

            return _items;
        }
    }

    public static List<SkillInfo> Skills
    {
        get
        {
            if (_skills.Count == 0) SetSkills();

            return _skills;
        }
    }

    private static void SetHds()
    {
        var lines = File.ReadAllText(Environment.CurrentDirectory + "/Resource/HardDrives.txt", Encoding.UTF8);
        var objc = JsonConvert.DeserializeObject<List<HdInfo>>(lines);
        _hds.AddRange(objc);
    }

    private static void SetMobos()
    {
        var lines = File.ReadAllText(Environment.CurrentDirectory + "/Resource/Mobos.txt", Encoding.UTF8);
        var objc = JsonConvert.DeserializeObject<List<MoboInfo>>(lines);
        _mobos.AddRange(objc);
    }

    private static void SetItems()
    {
        var itemList = File.ReadAllBytes(Environment.CurrentDirectory + "/Resource/Itemlist.bin");


        for (int i = 0; i < itemList.Length; i++)
        {
            itemList[i] ^= 0x5A;
        }

        var size = Marshal.SizeOf<TItemList>();
        for (int i = 0; i < itemList.Length; i += size)
        {
            var currentBuffer = itemList.Skip(i).Take(size).ToArray();

            var pItemList = ByteArrayHelper.ByteArrayToStructure<TItemList>(currentBuffer, 0);

            var item = new ItemInfo()
            {
                Id = i / size,
                ItemListItem = pItemList,
                Type = GetItemType(pItemList.nUnique),
                Name = GetFixedName(pItemList.Name),
                ItemClass = GetItemClass(pItemList.stEffect)
            };
            _items.Add(item);
        }

        var defaultWeapon = new ItemInfo()
        {
            Id = 9998,
            Name = "Any Weapon",
            Type = ItemType.Weapon
        };
        var defaultArmor = new ItemInfo()
        {
            Id = 9999,
            Name = "Any Armor",
            Type = ItemType.Armor
        };

        _items.Add(defaultArmor);
        _items.Add(defaultWeapon);
    }

    private static string GetFixedName(byte[] name)
    {
        int i = name.Length - 1;
        while (name[i] == 0 || name[i] == 254)
        {
            --i;
            if (i < 0)
                break;
        }

        if (i <= 0) return "";

        byte[] bar = new byte[i + 1];
        Array.Copy(name, bar, i + 1);

        return Encoding.Latin1.GetString(bar);
    }

    private static ItemClass GetItemClass(TStaticEffect[] stEffect)
    {
        var ef = stEffect.FirstOrDefault(x => x.sEffect == 87);
        return (ItemClass)ef.sValue;
    }

    private static ItemType GetItemType(int nUnique)
    {
        if (nUnique is > 1 and < 50)
            return ItemType.Armor;
        if (nUnique is > 50 and < 200) return ItemType.Weapon;

        return ItemType.Item;
    }

    private static void SetSkills()
    {
        _skills.Add(new SkillInfo
        {
            Name = "Ranged",
            Id = GameSkills.Ranged,
            Aggressive = true,
            MaxTarget = 1,
            Delay = 0.15d,
            Range = 5,
            MobClass = MobClass.ALL
        });

        _skills.Add(new SkillInfo
        {
            Name = "Melee",
            Id = GameSkills.Mlee,
            Aggressive = true,
            MaxTarget = 1,
            Delay = 0.15d,
            Range = 2,
            MobClass = MobClass.ALL
        });

        var lines = File.ReadAllText(Environment.CurrentDirectory + "/Resource/SkillData.json");

        var skills = JsonConvert.DeserializeObject<List<SkillInfo>>(lines);

        if (skills != null) _skills.AddRange(skills);
    }

    public static string GetItemName(int itemId)
    {
        return _items[itemId].Name ?? "N/A";
    }

    public static Image<Rgba32> CutHMap(AreaRect rect)
    {
        var imageClone = HeightMapImage.Clone(x =>
        {
            var width = rect.Max.X - rect.Min.X;
            var height = rect.Max.Y - rect.Min.Y;
            var cropRect = new Rectangle(rect.Min.X, rect.Min.Y, width, height);
            x.Crop(cropRect);
        });

        return imageClone;
    }
}