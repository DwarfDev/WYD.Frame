using System.Runtime.InteropServices;

namespace WYD.Frame.Conversors;

public class ItemListByItemName
{
    string ByteArrayToHexString(byte[] ba)
    {
        return BitConverter.ToString(ba).Replace("-", " ");
    }
    public ItemListByItemName()
    {
        
        var file = Environment.CurrentDirectory + "/Resource/Itemname.bin";


        var bytesFile = File.ReadAllBytes(file);

        int remainingSize = bytesFile.Length;
        var size = Marshal.SizeOf<ExtractIcon.ItemName>();
        List<ExtractIcon.ItemName> itemNames = new();
        int aux = 0;
        while (true)
        {
            var currentBytes = bytesFile.Skip(aux * size).Take(size).ToArray();

            if (currentBytes.Length < size) break;
            var stru = ExtractIcon.Pak.ToStruct<ExtractIcon.ItemName>(currentBytes);
    
            for (int nTemp = 0; nTemp < 62; ++nTemp)
            {
                stru.Name[nTemp] -= (byte)nTemp;
            }

            itemNames.Add(stru);
    
            remainingSize -= size;
            aux++;
        }

        var currentItemList = File.ReadAllLines(Environment.CurrentDirectory + "/Resource/ItemList.csv");

        for (int i =0; i < currentItemList.Length; i++)
        {
            var split = currentItemList[i].Split(",");

            var id = int.Parse(split[0]);
    
            if(itemNames.All(x => x.Id != id))
                continue;
            ExtractIcon.ItemName? idFromName = itemNames.First(x => x.Id == id);
    
            var name = idFromName.Value.GetName().Replace("\0", String.Empty);
    
            currentItemList[i] = currentItemList[i].Replace(split[1], name.Replace(" ", ""));
        }

        File.WriteAllLines(Environment.CurrentDirectory + "/Resource/ItemList_new.csv", currentItemList);

    }
}