using WYD.Frame.Common.Enum;
using WYD.Frame.Common.Enum.Game;

namespace WYD.Frame.Game.Workers;

public abstract class WorkerBase
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    protected readonly WClient WClientOwner;

    public WorkState State = WorkState.Created;
    protected bool IsPassive = false;
    public bool SyncRun { get; set; }= false;

    public WorkerBase(WClient wClientOwner)
    {
        WClientOwner = wClientOwner;
        WClientOwner.Works.Add(this);
        
    }

    public string Id { get; set; } = Guid.NewGuid().ToString();

    protected abstract void DoWork();

    public virtual void Notify(object? args)
    {
        
    }

    public void Pause()
    {
        State = WorkState.Paused;
    }

    public void Resume()
    {
        State = WorkState.Running;
    }

    public virtual void Stop()
    {
        WClientOwner.Works.WorkerRemoved?.Invoke(WClientOwner, this);
        State = WorkState.Ended;
        _cancellationTokenSource.Cancel();
        WClientOwner.Works.Remove(Id);
    }


    private void Execute()
    {
        if (IsPassive)
        {
            return;
        }

        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            Thread.Sleep(5);

            try
            {
                if (State == WorkState.Paused)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                if (WClientOwner.Status != ConnectionStatus.World)
                {
                    Stop();
                    continue;
                }

                DoWork();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
    public virtual void Start()
    {
        WClientOwner.Works.WorkerCreated?.Invoke(WClientOwner, this);
        
        if (SyncRun)
        {
            Execute();
        }
        else
        {
            Task.Run(Execute);
        }
    }
}