using WYD.Frame.Common.Enum;
using WYD.Frame.Game.Workers;

namespace WYD.Frame.Game;

public class Works
{
    private readonly WClient _wClient;
    private readonly object _workerLock = new();

    private readonly List<WorkerBase> _workers = new();

    public Works(WClient wClient)
    {
        _wClient = wClient;
    }

    public EventHandler<WorkerBase>? WorkerRemoved { get; set; }
    public EventHandler<WorkerBase>? WorkerCreated { get; set; }

    internal void Remove(string id)
    {
        lock (_workerLock)
        {
            var targetWorker = _workers.FirstOrDefault(x => x.Id.Equals(id));

            if (targetWorker is null) return;

            _workers.Remove(targetWorker);
        }
    }
    
    public void Stop(string id)
    {
        lock (_workerLock)
        {
            var targetWorker = _workers.FirstOrDefault(x => x.Id.Equals(id));

            if (targetWorker is null) return;

            targetWorker.Stop();
        }
    }

    public WorkerBase Get(string id)
    {
        lock (_workerLock)
        {
            return _workers.First(x => x.Id.Equals(id));
        }
    }
    public IEnumerable<WorkerBase> GetAll()
    {
        lock (_workerLock)
        {
            return _workers.ToList();
        }
    }
    public void Add(WorkerBase worker)
    {
        lock (_workerLock)
        {
            _workers.Add(worker);
        }
    }

    public void PauseAll(string? ignored = null)
    {
        _wClient.Log(MessageRelevance.Medium, $"Pausing all macros except for {ignored}.");
        lock (_workerLock)
        {
            _workers.ForEach(x =>
            {
                if (ignored is null || !x.Id.Equals(ignored))
                    x.Pause();
            });
        }
    }

    public void ResumeAll()
    {
        _wClient.Log(MessageRelevance.Medium, $"Resuming all macros.");
        lock (_workerLock)
        {
            _workers.ForEach(x => x.Resume());
        }
    }

    public bool Contains(string id)
    {
        lock (_workerLock)
        {
            return _workers.Any(x => x.Id.Equals(id));
        }
    }
    
    public bool Contains<T>()
    {
        lock (_workerLock)
        {
            return _workers.Any(x => x.GetType() == typeof(T));
        }
    }

    public void StopAll()
    {
        lock (_workerLock)
        {
            _workers.ForEach(work => work.Stop());
        }
    }
}