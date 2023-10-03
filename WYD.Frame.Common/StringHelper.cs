using System.Text;

namespace WYD.Frame.Common;

public static class StringHelper
{
    public static byte[] StringToByteArray(string hex)
    {
        return Enumerable.Range(0, hex.Length)
            .Where(x => x % 2 == 0)
            .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
            .ToArray();
    }
    public static byte[] TrimTailingZeros(this byte[] arr)
    {
        if (arr == null || arr.Length == 0)
            return arr;
        return arr.Reverse().SkipWhile(x => x == 0).Reverse().ToArray();
    }

    public static string ByteArrayToHexString(byte[] ba)
    {
        return BitConverter.ToString(ba).Replace("-", " ");
    }

    public static string ByteArrayToString(byte[] buffer)
    {
        var count = Array.IndexOf<byte>(buffer, 0, 0);
        if (count < 0) count = buffer.Length;
        return Encoding.ASCII.GetString(buffer, 0, count);
    }
    
    public static int Operate(this char logic, string x, string y){
        int xOp;
        int yOp;

        if (!int.TryParse(x, out xOp) || !int.TryParse(y, out yOp))
            throw new Exception("Falha ao identificar operador matemático");
            
        switch(logic){
            case '+':
                return xOp + yOp;
            case 'x':
                return xOp * yOp;
            case '-':
                return xOp - yOp;
            default:
                throw new InvalidOperationException();
        }
    }
}