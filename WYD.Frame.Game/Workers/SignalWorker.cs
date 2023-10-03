using WYD.Frame.Common.Enum.Game;

namespace WYD.Frame.Game.Workers;

public class SignalWorker : WorkerBase
{
    private long _lastSentTime = 0;

    public SignalWorker(WClient wClientOwner) : base(wClientOwner)
    {
    }

    protected override void DoWork()
    {
        if(WClientOwner.Status == ConnectionStatus.World)
            WClientOwner.Timer.TickSignal();
        // if (ClientOwner.Status == ConnectionStatus.World)
        // {
        //     if (ClientOwner.CommunicationControl.CurrentTime  >= _lastSentTime + 250000 || _lastSentTime == 0)
        //     {
        //         ClientOwner.SendSignal();
        //         _lastSentTime = ClientOwner.CommunicationControl.CurrentTime;
        //     }
        // }
        Thread.Sleep(100);
    }
}