using System.Runtime.InteropServices;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace WYD.Frame.Conversors;

public class ExtractIcon
{
    public ExtractIcon()
    {
        var itemIconLines = File.ReadAllLines(Environment.CurrentDirectory + "/Resource/Itemicon.csv");

        foreach (var line in itemIconLines)
        {
            if (line.StartsWith("#")) continue;

            var lineSplit = line.Split(",");
            var itemId = int.Parse(lineSplit[0]);
            var gridPos = int.Parse(lineSplit[1]);

            if (gridPos == 0 || gridPos == -1) continue;
            int imageId;

            if (gridPos < 100)
                imageId = 1;
            else if (gridPos % 100 == 0)
            {
                imageId = gridPos / 100 + 1;
            }
            else
            {
                var num = gridPos;
                imageId = 0;
                while (num > 0)
                {
                    num = num - 100;
                    imageId++;
                }
            }

            if (!File.Exists(Environment.CurrentDirectory +
                             $"/Resource/Tgs/itemicon{imageId.ToString().PadLeft(2, '0')}.tga")) continue;

            var file = Image<Rgba32>.Load(Environment.CurrentDirectory +
                                          $"/Resource/Tgs/itemicon{imageId.ToString().PadLeft(2, '0')}.tga");

            var savePath = Environment.CurrentDirectory + $"/Resource/Icon";

            file.Mutate(i =>
            {
                var width = file.Width;
                var height = file.Height;

                if (imageId > 0)
                    imageId--;

                var diff = (gridPos - (imageId * 100));
                var row = diff / 10;
                var col = diff % 10;

                if (col > 0)
                    col--;

                i.Crop(new Rectangle(new Point((col) * 35, row * 35), new Size(35, 35)));
            });

            file.SaveAsWebp(savePath + $"/{itemId}.webp");
            Console.WriteLine($"{itemId} = {gridPos}");
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct ItemName
    {
        public int Id;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        public byte[] Name;


        public string GetName()
        {
            return Encoding.Latin1.GetString(Name);
        }
    }

    public static class Pak
    {
        public static T ToStruct<T>(byte[] Data)
        {
            unsafe
            {
                fixed (byte* pBuffer = Data)
                {
                    return (T)Marshal.PtrToStructure(new IntPtr((void*)&pBuffer[0]), typeof(T));
                }
            }
        }

        public static T ToStruct<T>(byte[] Data, int Start)
        {
            unsafe
            {
                fixed (byte* pBuffer = Data)
                {
                    return (T)Marshal.PtrToStructure(new IntPtr((void*)&pBuffer[Start]), typeof(T));
                }
            }
        }

        public static byte[] ToByteArray<T>(T Struct)
        {
            byte[] Data = new byte[Marshal.SizeOf(Struct)];

            unsafe
            {
                fixed (byte* Buffer = Data)
                {
                    Marshal.StructureToPtr(Struct, new IntPtr((void*)Buffer), true);
                }
            }

            return Data;
        }
    }
}