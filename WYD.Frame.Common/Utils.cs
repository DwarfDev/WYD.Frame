using System.Text;

namespace WYD.Frame.Common;

public static class Utils
{
    public static Random Rand = new Random();
    
    public static T? GetAttribute<T>(this System.Enum value) where T : Attribute {
        var type = value.GetType();
        var memberInfo = type.GetMember(value.ToString());
        var attributes = memberInfo[0].GetCustomAttributes(typeof(T), false);
        return attributes.Length > 0 
            ? (T)attributes[0]
            : null;
    }
    public static int GetCorrectRationForCurrentMount(int mountId)
    {
        switch (mountId)
        {
            case 2360: return 2420; //,Porco,947.0,0,0.0.0
            case 2361: return 2421; //,Javali,948.0,0,60.0
            case 2362: return 2422; //,Lobo,949.0,0,80.0.0
            case 2363: return 2423; //,Dragão_Menor,950.0,
            case 2364: return 2424; //,Urso,951.0,0,100.0.
            case 2365: return 2425; //,Dente_de_Sabre,952.
            case 2366: return 2426; //,Cavalo_s / Sela_N,953
            case 2367: return 2426; //,Cavalo_Fantasm_N,95
            case 2368: return 2426; //,Cavalo_Leve_N,953.0
            case 2369: return 2426; //,Cavalo_Equip_N,953.
            case 2370: return 2426; //,Andaluz_N,953.0,0,2
            case 2371: return 2426; //,Cavalo_s / Sela_B,954
            case 2372: return 2426; //,Cavalo_Fantasm_B,95
            case 2373: return 2426; //,Cavalo_Leve_B,954.0
            case 2374: return 2426; //,Cavalo_Equip_B,954.
            case 2375: return 2426; //,Andaluz_B,954.0,0,2
            case 2376: return 2436; //,Fenrir,955.0,0,200.
            case 2377: return 2437; //,Dragão,956.0,0,200.
            case 2378: return 2438; //,Fenrir_das_Sombras,
            case 2379: return 2427; //,Tigre_de_Fogo,314.0
            case 2380: return 2428; //,Dragão_Vermelho,315
            case 2381: return 2429; //,Unicórnio,312.0,0,2
            case 2382: return 2429; //,Pegasus,312.0,0,219
            case 2383: return 2429; //,Unisus,312.0,0,219.
            case 2384: return 2430; //,Grifo,313.0,0,255.0
            case 2385: return 2430; //,Hipogrifo,313.0,0,2
            case 2386: return 2430; //,Grifo_Sangrento,313
            case 2387: return 2431; //,Svadilfari,953.0,0,
            case 2388: return 2432; //,Sleipnir,312.0,0,21
            case 2389: return 2439; //,Pantera_Negra,952.0
            default: return -1;
        }
    }
    
    public static string GenerateHwid()
    {
        var random = new Random();
        const string chars = "0123456789";
        var randomString = new string(Enumerable.Repeat(chars, 12)
            .Select(s => s[random.Next(s.Length)]).ToArray());

        var formattedString = string.Join(string.Empty,
            randomString.Select((x, i) => i > 0 && i % 2 == 0 ? string.Format(":{0}", x) : x.ToString()));

        var resultBytes = Encoding.UTF8.GetBytes(formattedString);
        var tmpBuffer = new byte[17];

        resultBytes.CopyTo(tmpBuffer, 0);

        return Encoding.UTF8.GetString(resultBytes);
    }

    public static T Clone<T>(this T val) where T : struct
    {
        return val;
    }
}