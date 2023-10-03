using System.Drawing;
using WYD.Frame.Common.Enum;
using WYD.Frame.Models.Models;
using WYD.Frame.Services.Models.Lan;

namespace WYD.Frame.Services;

public class QuestService
{
    private static List<Quest>? _lans = null;
    private static List<Quest> Quests
    {
        get
        {
            if (_lans is null)
            {
                _lans = LoadQuests();
            }

            return _lans;
        }
    }

    private static List<Quest> LoadQuests()
    {
        var quests = new List<Quest>();
        
        //LanN
        
        quests.Add(new()
        {
            BlockSize = new AreaSize(15,15),
            LowerLimit = new Position
            {
                X = 3601,
                Y = 3601
            },
            UpperLimit = new Position
            {
                X = 3693,
                Y = 3693
            },
            Type = QuestType.LanN
        });
        
        //Coveiro

        quests.Add(new Quest()
        {
            BlockSize = new AreaSize(15,15),
            UpperLimit = new Position(2426,2132),
            LowerLimit = new Position(2379,2077),
            Type = QuestType.Coveiro
        });
        
        
        //Colheita
        
        quests.Add(new Quest()
        {
            BlockSize = new AreaSize(14,14),
            UpperLimit = new Position(2257, 1728),
            LowerLimit = new Position(2228, 1700),
            Type = QuestType.Jardim
        });
        
        //Kaizen
        quests.Add(new Quest()
        {
            BlockSize = new AreaSize(12,14),
            UpperLimit = new Position(497, 3916),
            LowerLimit = new Position(459, 3887),
            Type = QuestType.Kaizen
        });
        
        //Hidra
        quests.Add(new Quest()
        {
            BlockSize = new AreaSize(16,24),
            UpperLimit = new Position(705, 3764),
            LowerLimit = new Position(656, 3727),
            Type = QuestType.Hidra
        });
        
        
        return quests;
    }
  
    public static Quest GetQuestByType(QuestType questType)
    {
        return Quests.First(x => x.Type == questType);
    }
    public static List<Quest> GetAllQuests()
    {
        return Quests.ToList();
    }
    
    
}