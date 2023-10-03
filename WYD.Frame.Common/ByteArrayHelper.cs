using System.Runtime.InteropServices;

namespace WYD.Frame.Common;

public class ByteArrayHelper
{
    public static byte[] StructureToByteArray<T>(T structure)
    {
        var buffer = new byte[Marshal.SizeOf(typeof(T))];

        unsafe
        {
            fixed (byte* pBuffer = buffer)
            {
                Marshal.StructureToPtr(structure ?? throw new ArgumentNullException(nameof(structure)),
                    new nint(pBuffer), true);
            }
        }

        return buffer;
    }

    public static T ByteArrayToStructure<T>(byte[] buffer, int offset)
    {
        unsafe
        {
            fixed (byte* pBuffer = buffer)
            {
                return ((T)Marshal.PtrToStructure(new nint(&pBuffer[offset]), typeof(T))!)!;
            }
        }
    }
}