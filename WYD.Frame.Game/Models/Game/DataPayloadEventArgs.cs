namespace WYD.Frame.Game.Models.Game;

public class DataPayloadEventArgs
{
    public byte[] Buffer { get; set; } = new byte[2];
    public byte[] EncBuffer { get; set; } = new byte[2];
}