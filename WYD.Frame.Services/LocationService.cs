using WYD.Frame.Common.Enum;
using WYD.Frame.Models.Models;
using WYD.Frame.Packets;
using WYD.Frame.Services.Common;
using WYD.Frame.Services.Models;
using WYD.Frame.Services.Models.Location;

namespace WYD.Frame.Services;

public static class LocationService
{
    private static readonly List<HuntingLead> HuntingScrolls = new()
    {
        new HuntingLead
        {
            ItemId = 3432,
            Leads = new List<Position>
            {
                new() { X = 2370, Y = 2106 }, new() { X = 2508, Y = 2101 },
                new() { X = 2526, Y = 2009 }, new() { X = 2609, Y = 2001 },
                new() { X = 2126, Y = 1600 }, new() { X = 2005, Y = 1617 },
                new() { X = 2241, Y = 1474 }, new() { X = 1858, Y = 1721 },
                new() { X = 2250, Y = 1316 }, new() { X = 1989, Y = 1755 }
            }
        },
        new HuntingLead
        {
            ItemId = 3433,
            Leads = new List<Position>
            {
                new() { X = 290, Y = 3799 }, new() { X = 724, Y = 3781 },
                new() { X = 481, Y = 4062 }, new() { X = 876, Y = 4058 },
                new() { X = 855, Y = 3922 }, new() { X = 808, Y = 3876 },
                new() { X = 959, Y = 3813 }, new() { X = 926, Y = 3750 },
                new() { X = 1096, Y = 3730 }, new() { X = 1132, Y = 3800 }
            }
        },
        new HuntingLead
        {
            ItemId = 3435,
            Leads = new List<Position>
            {
                new() { X = 1376, Y = 1722 }, new() { X = 1426, Y = 1686 },
                new() { X = 1381, Y = 1861 }, new() { X = 1326, Y = 1896 },
                new() { X = 1510, Y = 1723 }, new() { X = 1543, Y = 1726 },
                new() { X = 1580, Y = 1758 }, new() { X = 1182, Y = 1714 },
                new() { X = 1634, Y = 1727 }, new() { X = 1237, Y = 1764 }
            }
        },
        new HuntingLead
        {
            ItemId = 3434,
            Leads = new List<Position>
            {
                new() { X = 1242, Y = 4035 }, new() { X = 1264, Y = 4017 },
                new() { X = 1333, Y = 3994 }, new() { X = 1358, Y = 4041 },
                new() { X = 1462, Y = 4033 }, new() { X = 1326, Y = 3788 },
                new() { X = 1493, Y = 3777 }, new() { X = 1437, Y = 3741 },
                new() { X = 1389, Y = 3740 }, new() { X = 1422, Y = 3810 }
            }
        },
        new HuntingLead
        {
            ItemId = 3436,
            Leads = new List<Position>
            {
                new() { X = 2367, Y = 4024 }, new() { X = 2236, Y = 4044 },
                new() { X = 2236, Y = 3993 }, new() { X = 2209, Y = 3989 },
                new() { X = 2453, Y = 4067 }, new() { X = 2485, Y = 4043 },
                new() { X = 2534, Y = 3897 }, new() { X = 2489, Y = 3919 },
                new() { X = 2269, Y = 3910 }, new() { X = 2202, Y = 3866 }
            }
        },
        new HuntingLead
        {
            ItemId = 3437,
            Leads = new List<Position>
            {
                new() { X = 3664, Y = 3024 }, new() { X = 3582, Y = 3007 },
                new() { X = 3514, Y = 3008 }, new() { X = 3818, Y = 2977 },
                new() { X = 3517, Y = 2889 }, new() { X = 3745, Y = 2977 },
                new() { X = 3639, Y = 2877 }, new() { X = 3650, Y = 2727 },
                new() { X = 3660, Y = 2773 }, new() { X = 3746, Y = 2879 }
            }
        }
    };

    private static List<Area> _areas = new();

    public static List<Area> Areas
    {
        get
        {
            if (_areas.Count == 0)
            {
                GenerateAreas();
            }

            return _areas;
        }
    }

    private static void GenerateAreas()
    {
        /*
         *Teleporte entrada em armia > dungeon andar 1
            [2669, 2157]
            [2365, 2284]

            area 1 dungeon andar 1
            [137, 3723
            649, 3833]

            area 2 dungeon andar 1
            [650, 3776
            756, 3833]

            ligaçao area 1 e 2 dungeon andar 1
            [649, 3777]
            [650, 3777]

            area 3 dungeon andar 1
            [717, 3724
            756, 3775]

            ligaçao area 2 e 3 dungeon andar 1
            [720, 3775]
            [720, 3776]
         *
         * 
         */
        var areaInicial = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Area_Inicial,
            Bounds = new AreaRect(min: new Position(2063, 1935), max: new Position(2161, 2047))
        };

        var campoArmia_1 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Campo_Armia,
            Bounds = new AreaRect(min: new Position(2170, 2063), max: new Position(2374, 2160))
        };

        var questCoveiro = new Area()
        {
            Type = AreaType.Quest,
            Name = AreaNames.Quest_Coveiro,
            Bounds = new AreaRect(min: new Position(2379, 2077), max: new Position(2426, 2132))
        };

        var campoArmia_2 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Campo_Armia_2,
            Bounds = new AreaRect(min: new Position(2375, 2134), max: new Position(2447, 2160))
        };

        var campoArmia_3 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Campo_Armia_3,
            Bounds = new AreaRect(min: new Position(2319, 2161), max: new Position(2417, 2289))
        };

        var campoErion = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Campo_Erion,
            Bounds = new AreaRect(min: new Position(2448, 2024), max: new Position(2673, 2161))
        };

        var campoErion_2 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Campo_Erion_2,
            Bounds = new AreaRect(min: new Position(2477, 1968), max: new Position(2673, 2023))
        };

        var campoErion_3 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Campo_Erion_3,
            Bounds = new AreaRect(min: new Position(2447, 1935), max: new Position(2673, 1967))
        };

        var campoAzran_2 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Campo_Azran_2,
            Bounds = new AreaRect(min: new Position(2447, 1751), max: new Position(2545, 1934))
        };

        var campoAzran_3 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Campo_Azran_3,
            Bounds = new AreaRect(min: new Position(2191, 1549), max: new Position(2433, 1778))
        };

        var campoAzran_4 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Campo_Azran_4,
            Bounds = new AreaRect(min: new Position(2176, 1167), max: new Position(2303, 1548))
        };

        var campoAzran_5 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Campo_Azran_5,
            Bounds = new AreaRect(min: new Position(1800, 1549), max: new Position(2190, 1791))
        };

        var reinos = new Area()
        {
            Type = AreaType.City,
            Name = AreaNames.Reinos,
            Bounds = new AreaRect(min: new Position(1679, 1550), max: new Position(1799, 1918))
        };

        var deserto = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Deserto,
            Bounds = new AreaRect(min: new Position(1076, 1677), max: new Position(1678, 1918))
        };

        var campoGelo = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Campo_Gelo,
            Bounds = new AreaRect(min: new Position(3456, 2687), max: new Position(3967, 3085))
        };

        var submundoPrimeiroAndar_1 = new Area()
        {
            Type = AreaType.Submundo,
            Name = AreaNames.Submundo_Primeiro_Andar_1,
            Bounds = new AreaRect(min: new Position(1165, 3981), max: new Position(1281, 4082))
        };

        var submundoPrimeiroAndar_2 = new Area()
        {
            Type = AreaType.Submundo,
            Name = AreaNames.Submundo_Primeiro_Andar_2,
            Bounds = new AreaRect(min: new Position(1282, 3981), max: new Position(1356, 4018))
        };

        var submundoPrimeiroAndar_3 = new Area()
        {
            Type = AreaType.Submundo,
            Name = AreaNames.Submundo_Primeiro_Andar_3,
            Bounds = new AreaRect(min: new Position(1282, 4063), max: new Position(1356, 4082))
        };

        var submundoPrimeiroAndar_4 = new Area()
        {
            Type = AreaType.Submundo,
            Name = AreaNames.Submundo_Primeiro_Andar_4,
            Bounds = new AreaRect(min: new Position(1357, 3981), max: new Position(1523, 4082))
        };

        var submundoSegundoAndar_1 = new Area()
        {
            Type = AreaType.Submundo,
            Name = AreaNames.Submundo_Segundo_Andar_1,
            Bounds = new AreaRect(min: new Position(1294, 3726), max: new Position(1520, 3826))
        };

        var questElfos = new Area()
        {
            Type = AreaType.Quest,
            Name = AreaNames.Quest_Elfos,
            Bounds = new AreaRect(min: new Position(1283, 4020), max: new Position(1354, 4051))
        };


        var dungeonSegundoAndar_1 = new Area()
        {
            Type = AreaType.Dungeon,
            Name = AreaNames.Dungeon_Segundo_Andar_1,
            Bounds = new AreaRect(min: new Position(834, 3984), max: new Position(1012, 4084))
        };

        var dungeonSegundoAndar_2 = new Area()
        {
            Type = AreaType.Dungeon,
            Name = AreaNames.Dungeon_Segundo_Andar_2,
            Bounds = new AreaRect(min: new Position(774, 3984), max: new Position(833, 4038))
        };

        var dungeonSegundoAndar_3 = new Area()
        {
            Type = AreaType.Dungeon,
            Name = AreaNames.Dungeon_Segundo_Andar_3,
            Bounds = new AreaRect(min: new Position(652, 3984), max: new Position(773, 4084))
        };

        var dungeonSegundoAndar_4 = new Area()
        {
            Type = AreaType.Dungeon,
            Name = AreaNames.Dungeon_Segundo_Andar_4,
            Bounds = new AreaRect(min: new Position(781, 3853), max: new Position(882, 3983))
        };

        var dungeonTerceiroAndar_1 = new Area()
        {
            Type = AreaType.Dungeon,
            Name = AreaNames.Dungeon_Terceiro_Andar_1,
            Bounds = new AreaRect(min: new Position(909, 3725), max: new Position(1139, 3827))
        };

        var dungeonPrimeiroAndar = new Area()
        {
            Type = AreaType.Dungeon,
            Name = AreaNames.Dungeon_Primeiro_Andar_1,
            Bounds = new AreaRect(min: new Position(137, 3723), max: new Position(649, 3833))
        };

        var dungeonPrimeiroAndar_2 = new Area()
        {
            Type = AreaType.Dungeon,
            Name = AreaNames.Dungeon_Primeiro_Andar_2,
            Bounds = new AreaRect(min: new Position(650, 3776), max: new Position(756, 3833))
        };

        var dungeonPrimeiroAndar_3 = new Area()
        {
            Type = AreaType.Dungeon,
            Name = AreaNames.Dungeon_Primeiro_Andar_3,
            Bounds = new AreaRect(min: new Position(717, 3724), max: new Position(756, 3775))
        };

        var dungeonPrimeiroAndar_4 = new Area()
        {
            Type = AreaType.Dungeon,
            Name = AreaNames.Dungeon_Primeiro_Andar_4,
            Bounds = new AreaRect(min: new Position(393, 3834), max: new Position(502, 3882))
        };

        var dungeonPrimeiroAndar_5 = new Area()
        {
            Type = AreaType.Dungeon,
            Name = AreaNames.Dungeon_Primeiro_Andar_5,
            Bounds = new AreaRect(min: new Position(393, 3883), max: new Position(453, 3920))
        };

        var dungeonPrimeiroAndar_6 = new Area()
        {
            Type = AreaType.Dungeon,
            Name = AreaNames.Dungeon_Primeiro_Andar_6,
            Bounds = new AreaRect(min: new Position(593, 3921), max: new Position(502, 4085))
        };

        var questHydra = new Area()
        {
            Type = AreaType.Quest,
            Name = AreaNames.Quest_Hydra,
            Bounds = new AreaRect(min: new Position(652, 3724), max: new Position(715, 3774))
        };

        var questKyzen = new Area()
        {
            Type = AreaType.Quest,
            Name = AreaNames.Quest_Kyzen,
            Bounds = new AreaRect(min: new Position(455, 3884), max: new Position(500, 3919))
        };

        var questGargula = new Area()
        {
            Type = AreaType.Quest,
            Name = AreaNames.Quest_Gargula,
            Bounds = new AreaRect(min: new Position(775, 4040), max: new Position(832, 4084))
        };

        var lanN = new Area()
        {
            Type = AreaType.Quest,
            Name = AreaNames.Quest_LanN,
            Bounds = new AreaRect(min: new Position(3598, 3598), max: new Position(3698, 3698))
        };

        var lanM = new Area()
        {
            Type = AreaType.Quest,
            Name = AreaNames.Quest_LanM,
            Bounds = new AreaRect(min: new Position(3726, 3470), max: new Position(3826, 3570))
        };

        var lanA = new Area()
        {
            Type = AreaType.Quest,
            Name = AreaNames.Quest_LanA,
            Bounds = new AreaRect(min: new Position(3854, 3598), max: new Position(3954, 3698))
        };

        var aguaN = new Area()
        {
            Type = AreaType.Quest,
            Name = AreaNames.Agua_N,
            Bounds = new AreaRect(min: new Position(1035, 3468), max: new Position(1134, 3567))
        };

        var aguaM = new Area()
        {
            Type = AreaType.Quest,
            Name = AreaNames.Agua_M,
            Bounds = new AreaRect(min: new Position(1163, 3595), max: new Position(1263, 3697))
        };

        var aguaA = new Area()
        {
            Type = AreaType.Quest,
            Name = AreaNames.Agua_A,
            Bounds = new AreaRect(min: new Position(1290, 3468), max: new Position(1391, 3567))
        };

        var evento = new Area()
        {
            Type = AreaType.Quest,
            Name = AreaNames.Evento,
            Bounds = new AreaRect(min: new Position(1038, 1423), max: new Position(1105, 1519)),
        };

        var evento_ponte = new Area()
        {
            Type = AreaType.Quest,
            Name = AreaNames.Evento_Ponte,
            Bounds = new AreaRect(min: new Position(1106, 1469), max: new Position(1121, 1473)),
        };


        //1119 1473
        var evento_inicio = new Area()
        {
            Type = AreaType.Quest,
            Name = AreaNames.Evento_Inicio,
            Bounds = new AreaRect(min: new Position(1122, 1423), max: new Position(1137, 1519)),
        };

        var azran = new Area()
        {
            Type = AreaType.City,
            Name = AreaNames.Azran,
            Bounds = new AreaRect(min: new Position(2434, 1603), max: new Position(2673, 1750)),
        };

        var armia = new Area()
        {
            Type = AreaType.City,
            Name = AreaNames.Armia,
            Bounds = new AreaRect(min: new Position(2063, 2048), max: new Position(2169, 2161))
        };

        var erion = new Area()
        {
            Type = AreaType.City,
            Name = AreaNames.Erion,
            Bounds = new AreaRect(min: new Position(2448, 1968), max: new Position(2476, 2023)),
            CommonGoods = new CommonGoods("Farche", new Position(2468, 2011),
                new AreaRect(new Position(2459, 2004), new Position(2468, 2011)), true)
        };

        var noatun = new Area()
        {
            Type = AreaType.City,
            Name = AreaNames.Noatun,
            Bounds = new AreaRect(min: new Position(1038, 1677), max: new Position(1075, 1777)),
        };

        var gelo = new Area()
        {
            Type = AreaType.City,
            Name = AreaNames.Gelo,
            Bounds = new AreaRect(min: new Position(3598, 3086), max: new Position(3698, 3163)),
        };

        var kefra_1 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Kefra_1,
            Bounds = new AreaRect(min: new Position(2304, 3984), max: new Position(2431, 4083))
        };

        var kefra_2 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Kefra_2,
            Bounds = new AreaRect(min: new Position(2195, 3860), max: new Position(2303, 4083))
        };

        var kefra_3 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Kefra_3,
            Bounds = new AreaRect(min: new Position(2304, 3864), max: new Position(2352, 3872))
        };

        var kefra_4 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Kefra_4,
            Bounds = new AreaRect(min: new Position(2353, 3863), max: new Position(2379, 3896))
        };

        var kefra_5 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Kefra_5,
            Bounds = new AreaRect(min: new Position(2380, 3863), max: new Position(2431, 3870))
        };

        var kefra_6 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Kefra_6,
            Bounds = new AreaRect(min: new Position(2432, 3863), max: new Position(2545, 4083))
        };

        var kefra_boss = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Kefra_Boss,
            Bounds = new AreaRect(min: new Position(2341, 3899), max: new Position(2391, 3953))
        };

        var uxmal = new Area()
        {
            Type = AreaType.City,
            Name = AreaNames.Uxmal,
            Bounds = new AreaRect(min: new Position(3209, 1673), max: new Position(3317, 1712))
        };

        var pista_0 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Pista_0,
            Bounds = new AreaRect(min: new Position(3333, 1601), max: new Position(3448, 1656))
        };

        var pista_1 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Pista_1,
            Bounds = new AreaRect(min: new Position(3338, 1545), max: new Position(3444, 1591))
        };

        var pista_2 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Pista_2,
            Bounds = new AreaRect(min: new Position(3337, 1411), max: new Position(3449, 1465))
        };

        var pista_3 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Pista_3,
            Bounds = new AreaRect(min: new Position(3333, 1029), max: new Position(3448, 1144))
        };

        var pista_4 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Pista_4,
            Bounds = new AreaRect(min: new Position(3334, 1285), max: new Position(3451, 1402))
        };

        var pista_5 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Pista_5,
            Bounds = new AreaRect(min: new Position(3333, 1157), max: new Position(3448, 1271))
        };

        var pista_6 = new Area()
        {
            Type = AreaType.Camp,
            Name = AreaNames.Pista_6,
            Bounds = new AreaRect(min: new Position(3332, 1471), max: new Position(3445, 1531))
        };

        #region portal_exits

        azran.Exits.Add(new AreaExit(noatun, new Position(2481, 1717), ExitType.Portal));
        erion.Exits.Add(new AreaExit(noatun, new Position(2457, 2018), ExitType.Portal));
        gelo.Exits.Add(new AreaExit(noatun, new Position(3649, 3109), ExitType.Portal));
        armia.Exits.Add(new AreaExit(noatun, new Position(2117, 2101), ExitType.Portal));

        noatun.Exits.Add(new AreaExit(armia, new Position(1046, 1725), ExitType.Portal));
        noatun.Exits.Add(new AreaExit(azran, new Position(1045, 1717), ExitType.Portal));
        noatun.Exits.Add(new AreaExit(erion, new Position(1045, 1709), ExitType.Portal));
        noatun.Exits.Add(new AreaExit(gelo, new Position(1053, 1709), ExitType.Portal));

        campoArmia_3.Exits.Add(new AreaExit(dungeonPrimeiroAndar, new Position(2365, 2284), ExitType.Portal));
        campoErion.Exits.Add(new AreaExit(dungeonPrimeiroAndar, new Position(2669, 2157), ExitType.Portal));
        dungeonPrimeiroAndar.Exits.Add(new AreaExit(campoArmia_3, new Position(145, 3789), ExitType.Portal));
        dungeonPrimeiroAndar.Exits.Add(new AreaExit(campoErion, new Position(146, 3773), ExitType.Portal));

        dungeonPrimeiroAndar_6.Exits.Add(new AreaExit(dungeonSegundoAndar_1, new Position(410, 4073), ExitType.Portal));
        dungeonSegundoAndar_1.Exits.Add(new AreaExit(dungeonPrimeiroAndar_6, new Position(1007, 4065),
            ExitType.Portal));

        dungeonPrimeiroAndar.Exits.Add(new AreaExit(dungeonSegundoAndar_1, new Position(147, 3781), ExitType.Portal));
        dungeonSegundoAndar_1.Exits.Add(new AreaExit(dungeonPrimeiroAndar, new Position(1006, 4030), ExitType.Portal));

        dungeonPrimeiroAndar_3.Exits.Add(new AreaExit(dungeonSegundoAndar_1, new Position(746, 3817), ExitType.Portal));
        dungeonSegundoAndar_1.Exits.Add(new AreaExit(dungeonPrimeiroAndar_3, new Position(1006, 3994),
            ExitType.Portal));

        dungeonSegundoAndar_3.Exits.Add(new AreaExit(dungeonTerceiroAndar_1, new Position(682, 4077), ExitType.Portal));
        dungeonTerceiroAndar_1.Exits.Add(new AreaExit(dungeonSegundoAndar_3, new Position(917, 3822), ExitType.Portal));

        dungeonSegundoAndar_4.Exits.Add(new AreaExit(dungeonTerceiroAndar_1, new Position(877, 3874), ExitType.Portal));
        dungeonTerceiroAndar_1.Exits.Add(new AreaExit(dungeonSegundoAndar_4, new Position(933, 3822), ExitType.Portal));

        campoAzran_5.Exits.Add(new AreaExit(submundoPrimeiroAndar_1, new Position(1825, 1775), ExitType.Portal));
        submundoPrimeiroAndar_1.Exits.Add(new AreaExit(campoAzran_5, new Position(1174, 4081), ExitType.Portal));

        submundoPrimeiroAndar_4.Exits.Add(new AreaExit(submundoSegundoAndar_1, new Position(1518, 3997),
            ExitType.Portal));
        submundoSegundoAndar_1.Exits.Add(new AreaExit(submundoPrimeiroAndar_4, new Position(1305, 3819),
            ExitType.Portal));

        kefra_4.Exits.Add(new AreaExit(kefra_boss, new Position(2366, 3892), ExitType.Portal));
        kefra_boss.Exits.Add(new AreaExit(uxmal, new Position(2366, 3925), ExitType.Portal));

        deserto.Exits.Add(new AreaExit(kefra_1, new Position(1314, 1901), ExitType.Portal));

        #endregion

        #region exits

        evento_inicio.Exits.Add(new AreaExit(evento_ponte, new Position(1122, 1470)));

        evento_ponte.Exits.Add(new AreaExit(evento_inicio, new Position(1121, 1470)));
        evento_ponte.Exits.Add(new AreaExit(evento, new Position(1106, 1470)));

        evento.Exits.Add(new AreaExit(evento_ponte, new Position(1105, 1470)));

        dungeonPrimeiroAndar.Exits.Add(new AreaExit(dungeonPrimeiroAndar_2, new Position(649, 3777)));
        dungeonPrimeiroAndar.Exits.Add(new AreaExit(dungeonPrimeiroAndar_2, new Position(649, 3777)));

        dungeonPrimeiroAndar.Exits.Add(new AreaExit(dungeonPrimeiroAndar_2, new Position(649, 3777)));
        dungeonPrimeiroAndar.Exits.Add(new AreaExit(dungeonPrimeiroAndar_4, new Position(412, 3833)));

        dungeonPrimeiroAndar_2.Exits.Add(new AreaExit(dungeonPrimeiroAndar, new Position(650, 3777)));
        dungeonPrimeiroAndar_2.Exits.Add(new AreaExit(dungeonPrimeiroAndar_3, new Position(720, 3775)));

        dungeonPrimeiroAndar_3.Exits.Add(new AreaExit(dungeonPrimeiroAndar_2, new Position(720, 3776)));

        dungeonPrimeiroAndar_4.Exits.Add(new AreaExit(dungeonPrimeiroAndar, new Position(412, 3834)));
        dungeonPrimeiroAndar_4.Exits.Add(new AreaExit(dungeonPrimeiroAndar_5, new Position(445, 3882)));

        dungeonPrimeiroAndar_5.Exits.Add(new AreaExit(dungeonPrimeiroAndar_4, new Position(445, 3883)));
        dungeonPrimeiroAndar_5.Exits.Add(new AreaExit(dungeonPrimeiroAndar_6, new Position(435, 3921)));

        dungeonPrimeiroAndar_6.Exits.Add(new AreaExit(dungeonPrimeiroAndar_5, new Position(435, 3920)));

        dungeonSegundoAndar_1.Exits.Add(new AreaExit(dungeonSegundoAndar_2, new Position(834, 4036)));
        dungeonSegundoAndar_1.Exits.Add(new AreaExit(dungeonSegundoAndar_4, new Position(878, 3984)));

        dungeonSegundoAndar_2.Exits.Add(new AreaExit(dungeonSegundoAndar_1, new Position(833, 4036)));
        dungeonSegundoAndar_2.Exits.Add(new AreaExit(dungeonSegundoAndar_3, new Position(774, 4036)));
        dungeonSegundoAndar_2.Exits.Add(new AreaExit(dungeonSegundoAndar_4, new Position(822, 3984)));

        dungeonSegundoAndar_3.Exits.Add(new AreaExit(dungeonSegundoAndar_2, new Position(773, 4036)));

        dungeonSegundoAndar_4.Exits.Add(new AreaExit(dungeonSegundoAndar_1, new Position(878, 3983)));
        dungeonSegundoAndar_4.Exits.Add(new AreaExit(dungeonSegundoAndar_2, new Position(822, 3983)));


        submundoPrimeiroAndar_1.Exits.Add(new AreaExit(submundoPrimeiroAndar_2, new Position(1281, 3994)));
        submundoPrimeiroAndar_1.Exits.Add(new AreaExit(submundoPrimeiroAndar_3, new Position(1281, 4074)));

        submundoPrimeiroAndar_2.Exits.Add(new AreaExit(submundoPrimeiroAndar_1, new Position(1282, 3994)));
        submundoPrimeiroAndar_2.Exits.Add(new AreaExit(submundoPrimeiroAndar_4, new Position(1356, 3997)));

        submundoPrimeiroAndar_3.Exits.Add(new AreaExit(submundoPrimeiroAndar_1, new Position(1282, 4074)));
        submundoPrimeiroAndar_3.Exits.Add(new AreaExit(submundoPrimeiroAndar_4, new Position(1356, 4076)));

        submundoPrimeiroAndar_4.Exits.Add(new AreaExit(submundoPrimeiroAndar_2, new Position(1357, 3997)));
        submundoPrimeiroAndar_4.Exits.Add(new AreaExit(submundoPrimeiroAndar_3, new Position(1357, 4076)));


        gelo.Exits.Add(new AreaExit(campoGelo, new Position(3648, 3086)));

        campoGelo.Exits.Add(new AreaExit(gelo, new Position(3658, 3085)));

        armia.Exits.Add(new AreaExit(campoArmia_1, new Position(2169, 2102)));
        armia.Exits.Add(new AreaExit(areaInicial, new Position(2112, 2048)));

        areaInicial.Exits.Add(new AreaExit(armia, new Position(2112, 2047)));

        campoArmia_1.Exits.Add(new AreaExit(armia, new Position(2170, 2102)));
        campoArmia_1.Exits.Add(new AreaExit(campoArmia_2, new Position(2374, 2143)));
        campoArmia_1.Exits.Add(new AreaExit(campoArmia_3, new Position(2368, 2160)));

        campoArmia_2.Exits.Add(new AreaExit(campoArmia_1, new Position(2375, 2143)));
        campoArmia_2.Exits.Add(new AreaExit(campoErion, new Position(2447, 2147)));

        campoArmia_3.Exits.Add(new AreaExit(campoArmia_1, new Position(2368, 2161)));


        campoErion.Exits.Add(new AreaExit(campoArmia_2, new Position(2448, 2147)));
        campoErion.Exits.Add(new AreaExit(campoErion_2, new Position(2548, 2024)));

        campoErion_2.Exits.Add(new AreaExit(campoErion, new Position(2548, 2023)));
        campoErion_2.Exits.Add(new AreaExit(erion, new Position(2477, 2010)));
        campoErion_2.Exits.Add(new AreaExit(campoErion_3, new Position(2586, 1968)));

        campoErion_3.Exits.Add(new AreaExit(campoErion_2, new Position(2586, 1967)));
        campoErion_3.Exits.Add(new AreaExit(campoAzran_2, new Position(2495, 1935)));

        erion.Exits.Add(new AreaExit(campoErion_2, new Position(2476, 2010)));

        campoAzran_2.Exits.Add(new AreaExit(campoErion_3, new Position(2495, 1934)));
        campoAzran_2.Exits.Add(new AreaExit(azran, new Position(2484, 1751)));

        campoAzran_3.Exits.Add(new AreaExit(azran, new Position(2433, 1718)));
        campoAzran_3.Exits.Add(new AreaExit(campoAzran_4, new Position(2256, 1549)));
        campoAzran_3.Exits.Add(new AreaExit(campoAzran_5, new Position(2191, 1596)));

        campoAzran_4.Exits.Add(new AreaExit(campoAzran_3, new Position(2256, 1548)));

        campoAzran_5.Exits.Add(new AreaExit(campoAzran_3, new Position(2190, 1596)));
        campoAzran_5.Exits.Add(new AreaExit(reinos, new Position(1800, 1726)));

        reinos.Exits.Add(new AreaExit(campoAzran_5, new Position(1799, 1726)));
        reinos.Exits.Add(new AreaExit(deserto, new Position(1679, 1726)));

        deserto.Exits.Add(new AreaExit(reinos, new Position(1678, 1726)));
        deserto.Exits.Add(new AreaExit(noatun, new Position(1076, 1711)));

        azran.Exits.Add(new AreaExit(campoAzran_2, new Position(2484, 1750)));
        azran.Exits.Add(new AreaExit(campoAzran_3, new Position(2434, 1718)));

        noatun.Exits.Add(new AreaExit(deserto, new Position(1075, 1711)));

        kefra_1.Exits.Add(new AreaExit(deserto, new Position(2366, 4073)));
        kefra_1.Exits.Add(new AreaExit(kefra_2, new Position(2303, 4065)));
        kefra_1.Exits.Add(new AreaExit(kefra_6, new Position(2431, 4066)));

        kefra_2.Exits.Add(new AreaExit(kefra_1, new Position(2304, 4065)));
        kefra_2.Exits.Add(new AreaExit(kefra_3, new Position(2303, 3867)));

        kefra_3.Exits.Add(new AreaExit(kefra_2, new Position(2304, 3867)));
        kefra_3.Exits.Add(new AreaExit(kefra_4, new Position(2352, 3867)));

        kefra_4.Exits.Add(new AreaExit(kefra_3, new Position(2353, 3867)));
        kefra_4.Exits.Add(new AreaExit(kefra_5, new Position(2379, 3867)));

        kefra_5.Exits.Add(new AreaExit(kefra_4, new Position(2380, 3867)));
        kefra_5.Exits.Add(new AreaExit(kefra_6, new Position(2431, 3867)));

        kefra_6.Exits.Add(new AreaExit(kefra_5, new Position(2432, 3867)));
        kefra_6.Exits.Add(new AreaExit(kefra_1, new Position(2432, 4066)));

        #endregion

        _areas.Add(armia);
        _areas.Add(gelo);
        _areas.Add(azran);
        _areas.Add(erion);
        _areas.Add(noatun);

        _areas.Add(evento);
        _areas.Add(evento_ponte);
        _areas.Add(evento_inicio);

        _areas.Add(questCoveiro);
        _areas.Add(questGargula);
        _areas.Add(questHydra);
        _areas.Add(questKyzen);
        _areas.Add(questElfos);

        _areas.Add(aguaN);
        _areas.Add(aguaM);
        _areas.Add(aguaA);

        _areas.Add(lanN);
        _areas.Add(lanM);
        _areas.Add(lanA);

        _areas.Add(submundoPrimeiroAndar_1);
        _areas.Add(submundoPrimeiroAndar_2);
        _areas.Add(submundoPrimeiroAndar_3);
        _areas.Add(submundoPrimeiroAndar_4);
        _areas.Add(submundoSegundoAndar_1);


        _areas.Add(dungeonPrimeiroAndar);
        _areas.Add(dungeonPrimeiroAndar_2);
        _areas.Add(dungeonPrimeiroAndar_3);
        _areas.Add(dungeonPrimeiroAndar_4);
        _areas.Add(dungeonPrimeiroAndar_5);
        _areas.Add(dungeonPrimeiroAndar_6);
        _areas.Add(dungeonSegundoAndar_1);
        _areas.Add(dungeonSegundoAndar_2);
        _areas.Add(dungeonSegundoAndar_3);
        _areas.Add(dungeonSegundoAndar_4);
        _areas.Add(dungeonTerceiroAndar_1);


        _areas.Add(campoGelo);
        _areas.Add(campoArmia_1);
        _areas.Add(campoArmia_2);
        _areas.Add(campoArmia_3);
        _areas.Add(campoErion);
        _areas.Add(campoErion_2);
        _areas.Add(campoErion_3);
        _areas.Add(deserto);
        _areas.Add(reinos);
        _areas.Add(campoAzran_2);
        _areas.Add(campoAzran_3);
        _areas.Add(campoAzran_4);
        _areas.Add(campoAzran_5);
        _areas.Add(areaInicial);

        _areas.Add(kefra_6);
        _areas.Add(kefra_5);
        _areas.Add(kefra_4);
        _areas.Add(kefra_3);
        _areas.Add(kefra_2);
        _areas.Add(kefra_1);
        _areas.Add(kefra_boss);
        _areas.Add(uxmal);
        _areas.Add(pista_0);
        _areas.Add(pista_1);
        _areas.Add(pista_2);
        _areas.Add(pista_3);
        _areas.Add(pista_4);
        _areas.Add(pista_5);
        _areas.Add(pista_6);
    }

    public static List<Area> AreaList { get; } = new()
    {
        new Area
        {
            Type = AreaType.City,
            Name = AreaNames.Armia,
            Portals = new List<Portal>
            {
                new()
                {
                    Location = new Position { X = 2117, Y = 2101 },
                    LeadsTo = AreaNames.Noatun
                },
                new()
                {
                    Location = new Position { X = 2141, Y = 2070 },
                    LeadsTo = AreaNames.Campo_Armia
                }
            }
        },
        new Area
        {
            Type = AreaType.City,
            Name = AreaNames.Noatun,
            Portals = new List<Portal>
            {
                new()
                {
                    Location = new Position { X = 1046, Y = 1725 },
                    LeadsTo = AreaNames.Armia
                },
                new()
                {
                    Location = new Position { X = 1045, Y = 1717 },
                    LeadsTo = AreaNames.Azran
                },
                new()
                {
                    Location = new Position { X = 1045, Y = 1709 },
                    LeadsTo = AreaNames.Erion
                },
                new()
                {
                    Location = new Position { X = 1053, Y = 1709 },
                    LeadsTo = AreaNames.Gelo
                }
            }
        },
        new Area
        {
            Type = AreaType.City,
            Name = AreaNames.Azran,
            Portals = new List<Portal>
            {
                new()
                {
                    Location = new Position { X = 2481, Y = 1717 },
                    LeadsTo = AreaNames.Noatun
                },
                new()
                {
                    Location = new Position { X = 2469, Y = 1717 },
                    LeadsTo = AreaNames.Campo_Azran_1
                },
                new()
                {
                    Location = new Position { X = 2454, Y = 1717 },
                    LeadsTo = AreaNames.Campo_Azran_2
                }
            }
        },
        new Area
        {
            Type = AreaType.City,
            Name = AreaNames.Erion,
            Portals = new List<Portal>
            {
                new()
                {
                    Location = new Position { X = 2457, Y = 2018 },
                    LeadsTo = AreaNames.Noatun
                },
                new()
                {
                    Location = new Position { X = 2463, Y = 1989 },
                    LeadsTo = AreaNames.Campo_Erion
                }
            }
        },
        new Area
        {
            Type = AreaType.City,
            Name = AreaNames.Gelo,
            Portals = new List<Portal>
            {
                new()
                {
                    Location = new Position { X = 3649, Y = 3109 },
                    LeadsTo = AreaNames.Noatun
                }
            }
        },
        new Area
        {
            Type = AreaType.Camp,
            Name = AreaNames.Campo_Armia,
            Portals = new List<Portal>()
        },
        new Area
        {
            Type = AreaType.Camp,
            Name = AreaNames.Campo_Azran_1,
            Portals = new List<Portal>()
        },
        new Area
        {
            Type = AreaType.Camp,
            Name = AreaNames.Campo_Azran_2,
            Portals = new List<Portal>()
        },
        new Area
        {
            Type = AreaType.Camp,
            Name = AreaNames.Campo_Erion,
            Portals = new List<Portal>()
        }
    };

    public static Area ClosestCity(Position position)
    {
        var closestCity = new Area();
        var smallestDistance = 6000; //Max between 0,0 and 4096,4096
        var firstVerification = true;
        for (var loop = 0; loop < AreaList.Count; loop++)
            if (AreaList[loop].Type == AreaType.City)
            {
                var iterationDistance =
                    position.GetDistance(AreaList[loop].CenterPosition);
                if (firstVerification)
                {
                    smallestDistance = iterationDistance;
                    closestCity = AreaList[loop];
                    firstVerification = false;
                }
                else
                {
                    if (iterationDistance < smallestDistance)
                    {
                        smallestDistance = iterationDistance;
                        closestCity = AreaList[loop];
                    }
                }
            }

        return closestCity;
    }

    public static Func<Area, IEnumerable<Area>> ShortestPathFunction(Area start)
    {
        var previous = new Dictionary<Area, Area>();

        var queue = new Queue<Area>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var vertex = queue.Dequeue();
            foreach (var neighbor in vertex.Exits)
            {
                if (previous.ContainsKey(neighbor.Destination))
                    continue;

                previous[neighbor.Destination] = vertex;
                queue.Enqueue(neighbor.Destination);
            }
        }

        Func<Area, IEnumerable<Area>> shortestPath = v =>
        {
            var path = new List<Area> { };

            var current = v;
            while (!current.Equals(start))
            {
                path.Add(current);
                current = previous[current];
            }

            ;

            path.Add(start);
            path.Reverse();

            return path;
        };

        return shortestPath;
    }

    public static HuntingScroll FindClosestScroll(Position pos)
    {
        var minRange = double.MaxValue;
        HuntingLead? minLead = null;
        var minIndex = 0;
        foreach (var hunting in HuntingScrolls)
        {
            var minimalRange = hunting.Leads.Select((x, index) =>
                new
                {
                    Index = index,
                    Position = x
                }).MinBy(x => x.Position.GetDistance(pos));

            if (minimalRange is null) continue;

            var range = minimalRange.Position.GetDistance(pos);
            if (range < minRange)
            {
                minRange = range;
                minLead = hunting;
                minIndex = minimalRange.Index;
            }
        }

        if (minLead is null) throw new Exception("Failed to find lead.");
        return new HuntingScroll
        {
            ItemId = minLead.ItemId,
            Index = minIndex,
            Range = minRange,
            Destination = HuntingScrolls.First(x => x.ItemId == minLead.ItemId).Leads[minIndex]
        };
    }

    public static Area? GetAreaHpa(Position pos)
    {
        foreach (var area in Areas)
        {
            if (area.Bounds.IsOnArea(pos))
                return area;
        }

        return null;
    }

    public static Area ClosestArea(Position pos)
    {
        var closestCity = new Area();
        var smallestDistance = 6000; //Max between 0,0 and 4096,4096
        var firstVerification = true;
        for (var loop = 0; loop < AreaList.Count; loop++)
        {
            var iterationDistance = pos.GetDistance(AreaList[loop].CenterPosition);
            if (firstVerification)
            {
                smallestDistance = iterationDistance;
                closestCity = AreaList[loop];
                firstVerification = false;
            }
            else
            {
                if (iterationDistance < smallestDistance)
                {
                    smallestDistance = iterationDistance;
                    closestCity = AreaList[loop];
                }
            }
        }

        return closestCity;
    }

    public static Area ClosestCommonGoods(Position pos, Area[] areasToSearch)
    {
        Area? closestCity = null;
        var smallestDistance = 6000; //Max between 0,0 and 4096,4096
        var firstVerification = true;
        for (var loop = 0; loop < areasToSearch.Length; loop++)
        {
            var iterationDistance = pos.GetDistance(areasToSearch[loop].CommonGoods.Position);
            if (firstVerification)
            {
                smallestDistance = iterationDistance;
                closestCity = areasToSearch[loop];
                firstVerification = false;
            }
            else
            {
                if (iterationDistance < smallestDistance)
                {
                    smallestDistance = iterationDistance;
                    closestCity = areasToSearch[loop];
                }
            }
        }

        if (closestCity is null) throw new Exception("Closest area not found.");

        return closestCity;
    }

    public static List<Area> DrawAreaPaths(Area from, Area to)
    {
        var arr = new List<Area>();
        var visitedArea = new List<Area>();

        if (from.Name == to.Name)
            return new List<Area>();

        if (HasExit(from, arr, visitedArea, to))
        {
            return arr;
        }

        return new List<Area>();
    }

    private static bool HasExit(Area? root,
        List<Area> arr, List<Area> visited, Area desiredArea)
    {
        if (root == null)
            return false;

        if (visited.Any(x => root == x)) return false;

        visited.Add(root);

        foreach (var areaExit in root.Exits)
        {
            arr.Add(areaExit.Destination);

            if (areaExit.Destination == desiredArea)
                return true;

            var areaLead = Areas.First(x => x == areaExit.Destination);
            if (HasExit(areaLead, arr, visited, desiredArea))
                return true;

            arr.RemoveAt(arr.Count - 1);
        }

        return false;
    }

    public static Portal[] DrawPathPortals(Area from, Area to)
    {
        var arr = new List<Portal>();
        var visitedArea = new List<Area>();

        if (from.Name == to.Name)
            return Array.Empty<Portal>();

        if (HasPath(from, arr, visitedArea, to.Name))
            return arr.ToArray();

        return Array.Empty<Portal>();
    }

    private static bool HasPath(Area? root,
        List<Portal> arr, List<Area> visited, AreaNames desiredArea)
    {
        if (root == null)
            return false;

        if (visited.Any(x => root.Name == x.Name)) return false;
        visited.Add(root);

        foreach (var portal in root.Portals)
        {
            arr.Add(portal);

            if (portal.LeadsTo == desiredArea)
                return true;

            var areaLead = AreaList.First(x => x.Name == portal.LeadsTo);
            if (HasPath(areaLead, arr, visited, desiredArea))
                return true;

            arr.RemoveAt(arr.Count - 1);
        }

        return false;
    }

    private class HuntingLead : HuntingScroll
    {
        public List<Position> Leads = new();
    }
}