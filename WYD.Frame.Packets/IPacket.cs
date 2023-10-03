using System.Runtime.InteropServices;
using WYD.Frame.Packets.Network;

namespace WYD.Frame.Packets;

public interface IPacket
{
    NetworkHeader Header { get; set; }
    
    public static T FromBytes<T>(byte[] bytes)
    {
        GCHandle gcHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
        var data = (T)Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), typeof(T))!;
        gcHandle.Free();
        return data;
    }

    public static byte[] ToBytes<T>(T data)
    {
        var size = Marshal.SizeOf(data);
        var byts = new byte[size];
        var ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(data!, ptr, true);
        Marshal.Copy(ptr, byts, 0, size);
        Marshal.FreeHGlobal(ptr);

        return byts;
    }
}