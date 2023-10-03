using WYD.Frame.Game.Contracts;
using WYD.Frame.Packets;
using WYD.Frame.Packets.Network;

namespace WYD.Frame.Game;

public class Timer
{
    private readonly WClient _wClient;
    private DateTime _lastSentTime = DateTime.Now.AddHours(-1);

    public Timer(WClient wClient)
    {
        _wClient = wClient;
    }

    public DateTime YellowEndsAt { get; private set; }
    public DateTime GreenEndsAt { get; private set; }

    internal void ReceiveYellowTime(P3A5_YellowTime yellowTimePacket)
    {
        YellowEndsAt = DateTime.Now.AddSeconds(yellowTimePacket.Seconds);
    }

    internal void ReceiveGreenTime(P3A1_GreenTime greenTimePacket)
    {
        GreenEndsAt = DateTime.Now.AddSeconds(greenTimePacket.Seconds);
    }

    /// <summary>
    ///     Sleep using server timestamp
    /// </summary>
    /// <param name="ms">Miliseconds to sleep</param>
    internal void Sleep(long ms)
    {
        var sentAt = _wClient.CommunicationControl.CurrentTime;

        while (_wClient.CommunicationControl.CurrentTime < sentAt + ms) Thread.Sleep(5);
    }

    internal void TickSignal()
    {
        var diff = DateTime.Now - _lastSentTime;
        if (diff.TotalMinutes >= 3)
        {
            _wClient.SendSignal();
            _lastSentTime = DateTime.Now;
        }
    }
}