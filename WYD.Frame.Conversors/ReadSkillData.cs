using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Mapster;
using Newtonsoft.Json;
using Reloaded.Memory.Sources;
using WYD.Frame.Common;
using WYD.Frame.Common.Enum.Game;
using WYD.Frame.Services.Models.Resource;

namespace WYD.Frame.Conversors;

public class ReadSkillData
{
    private readonly int MAX_SPELL = 248;

    public ReadSkillData()
    {
        var file = File.ReadAllBytes(Environment.CurrentDirectory + "/Resource/SkillData.bin");

        for (int i = 0; i < file.Length; i++)
        {
            file[i] = (byte)(file[i] ^ 0x5A);
        }

        var structSize = Marshal.SizeOf<GSpell>();
        var spellList = new List<SkillInfo>();
        var gameProc = Process.GetProcessesByName("Nix").First();
        IMemory externalMemory = new ExternalMemory(gameProc);

        if (gameProc.MainModule is null) return;
        
        var baseModule = gameProc.MainModule.BaseAddress;

        var baseSkillNames = (nuint) baseModule + 0x32E310;


        var current = 0;
        for (int i = 0; i < file.Length; i += structSize)
        {
            byte[] spellName;
            var tmpMemory = baseSkillNames+ (nuint) current * 144;
            externalMemory.Read(tmpMemory, out spellName, 66, false);

            var indexToTrim = spellName.ToList().FindIndex(z => z == 0);

            var spellNameStr = indexToTrim == -1 ? "Sem nome" : Encoding.Latin1.GetString(spellName).Substring(0, indexToTrim);
            
            var currentSpell = file.Skip(i).Take(structSize).ToArray();

            var currentSpellStruct = ByteArrayHelper.ByteArrayToStructure<GSpell>(currentSpell, 0);

            var adaptedStruct = currentSpellStruct.Adapt<SkillInfo>();
            adaptedStruct.Name = spellNameStr;
            adaptedStruct.Id = (GameSkills)current;
            adaptedStruct.MobClass = (MobClass)(current / 24);
            spellList.Add(adaptedStruct);
            current++;
        }


        var finalFile = Environment.CurrentDirectory + "/Resource/SkillData.json";
        
        File.WriteAllText(finalFile, JsonConvert.SerializeObject(spellList, Formatting.Indented));

    }

    public class GameSpell 
    {
        public GSpell Spell { get; set; }
        public string Name { get; set; }
        public GameSkills Id { get; set; }
    }
    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct GSpell
    {
        public int SkillPoint;
        public int TargetType;
        public int ManaSpent;
        public int Delay;
        public int Range;
        public int InstanceType;
        public int InstanceValue;
        public int TickType;
        public int TickValue;
        public int AffectType;
        public int AffectValue;
        public int AffectTime;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Act1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Act2;
        public int InstanceAttribute;
        public int TickAttribute;
        public int Aggressive;
        public int MaxTarget;
        public int bParty;
        public int AffectResist;
        public int Passive;
        public int ForceDamage;
    };

}