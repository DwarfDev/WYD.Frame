using System.Text;
using WYD.Frame.Common;
using WYD.Frame.Common.Enum;
using WYD.Frame.Common.Enum.Game;
using WYD.Frame.Game.Contracts;
using WYD.Frame.Game.Models.Game;
using WYD.Frame.Game.Pool;
using WYD.Frame.Game.Security;
using WYD.Frame.Game.Workers;
using WYD.Frame.Models.Models;
using WYD.Frame.Packets.Network;
using WYD.Frame.Services;

namespace WYD.Frame.Game;

public class WClient : IClient, IDisposable
{
    private readonly ServerInterpreter _interpreter;

    internal readonly ClientSocket Socket;
    public EventHandler<ConnectionStatus>? ConnectionChanged;

    public EventHandler<GameMessage>? LogMessageReceived;
    public EventHandler<Position>? Moved;
    public EventHandler<Player>? PlayerChanged;
    public EventHandler<PlayerStatus>? ScoreUpdated;


    public EventHandler<int>? PartyLeft;
    public EventHandler<NetworkParty>? PartyJoined;
    public EventHandler<NetworkParty>? PartyReceived;

    protected WClient(ClientConfiguration config)
    {
        Bag = new Bag(this);
        MobGrid = new MobGrid(this);
        Works = new Works(this);
        Social = new Social(this);
        Timer = new Timer(this);
        World = new World(this);
        Player = new Player(this);
        Charlist = new Charlist(this);
        Configuration = config;
        Socket = new ClientSocket(this);
        _interpreter = new ServerInterpreter(this);
        CommunicationControl = new CommunicationControl();

        Configure();
    }

    public static IClient Build(ClientConfiguration config)
    {
        var newClient = new WClient(config);
        
        GamePool.Add(newClient);
        
        return newClient;
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public MobGrid MobGrid { get; }
    public ConnectionStatus Status { get; internal set; }
    public Works Works { get; }
    public Bag Bag { get; }
    public Social Social { get; }
    public Timer Timer { get; }
    public World World { get; }
    public Player Player { get; }
    public Charlist Charlist { get; }
    public CommunicationControl CommunicationControl { get; }
    public ClientConfiguration Configuration { get; }

    public Queue<GameMessage> Logs { get; } = new();

   

    public void Log(MessageRelevance relevance, string msg)
    {
        var msgArgs = new GameMessage()
        {
            Message = msg,
            Command = "Log",
            Relevance = relevance
        };
        Logs.Enqueue(msgArgs);
        if (Logs.Count > 50)
            Logs.Dequeue();
        LogMessageReceived?.Invoke(this, msgArgs);
    }

    #region send

    /// <summary>
    ///     Initialize socket connection, may use proxy if configured.
    /// </summary>
    public async Task SendConnect()
    {
        await Socket.ConnectAsync();
        Socket.Send(StringHelper.StringToByteArray("11F3111F"));
    }

    /// <summary>
    ///     Send Packet with Client Credentials for account
    /// </summary>
    public void SendLogin()
    {
        var packet = P20D_Login.Create(Configuration.Credentials.Username, Configuration.Credentials.Password,
            Configuration.Credentials.Mac, Configuration.ConnectionConfiguration.ServerClientVersion);

        Socket.SendEncrypted(packet);
    }

    /// <summary>
    ///     Send client numeric
    /// </summary>
    public void SendNumeric()
    {
        var packet = PFDE_Numeric.Create(Configuration.Credentials.Numeric);

        Socket.SendEncryptedNumeric(packet);
    }

    /// <summary>
    ///     Send client hwid, testei isso uma vez e tava crashando o Nix...
    ///     Parecem que não descobriram o que era. Faltou log? Não sei
    ///     Talvez dê pra dupar, mas muito arriscado derrubar de novo... 
    /// </summary>
    public void SendHwidInfo()
    {
        var packet = PF00_Hwid.Create();

        var buffer = new List<byte>();
        var tresUnk = BitConverter.GetBytes((int)3);
        buffer.AddRange(tresUnk);
        var unkw1 = BitConverter.GetBytes(Utils.Rand.NextInt64(0x000000000000, 0xFFFFFFFFFFFF));
        buffer.AddRange(unkw1);
        byte[] unkw2 = new byte[1];
        buffer.AddRange(unkw2);
        var unkw3 = BitConverter.GetBytes(Utils.Rand.NextInt64(0x0000000000, 0xFFFFFFFFFF));
        buffer.AddRange(unkw3);
        byte[] unkw4 = new byte[7];
        buffer.AddRange(unkw4);
        byte[] unkw5 = BitConverter.GetBytes(2);
        buffer.AddRange(unkw5);

        byte[] hd = Encoding.UTF8.GetBytes(
            Convert.ToBase64String(Encoding.UTF8.GetBytes(Configuration.HwidInfo.HardDisk)));
        byte[] hdSize = BitConverter.GetBytes(hd.Length);
        buffer.AddRange(hdSize);
        buffer.AddRange(hd);

        byte[] umBool = BitConverter.GetBytes(1);
        buffer.AddRange(umBool);

        byte[] moboModel =
            Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(Configuration.HwidInfo.MoboName)));
        byte[] moboModelSize = BitConverter.GetBytes(moboModel.Length);
        buffer.AddRange(moboModelSize);
        buffer.AddRange(moboModel);

        byte[] moboManufacturer =
            Encoding.UTF8.GetBytes(
                Convert.ToBase64String(Encoding.UTF8.GetBytes(Configuration.HwidInfo.MoboManufacturer)));
        byte[] moboManufacturerSize = BitConverter.GetBytes(moboManufacturer.Length);
        buffer.AddRange(moboManufacturerSize);
        buffer.AddRange(moboManufacturer);

        byte[] quatro = BitConverter.GetBytes(4);
        buffer.AddRange(quatro);
        buffer.AddRange(Encoding.UTF8.GetBytes("x.x"));

        var header = Socket.GetBasicHeader(packet);
        header.Size = (ushort)(buffer.Count + 12);

        var headerBuffer = ByteArrayHelper.StructureToByteArray(header);

        var tmpBuffer = new byte[buffer.Count + headerBuffer.Length];
        headerBuffer.CopyTo(tmpBuffer, 0);

        buffer.CopyTo(tmpBuffer, 12);

        var bufferArr = tmpBuffer.ToArray();
        GameSecurity.Encrypt(ref bufferArr);

        Socket.Send(bufferArr);
    }

    /// <summary>
    ///     Send disconnection
    /// </summary>
    public void SendDisconnect()
    {
        GuildedService.Log(GuildedLogType.Behavior, $"Conta desconectada {Player.Name}", $"Shutdown",
            DateTime.Now.ToShortDateString());
        Socket.Dispose();
        Status = ConnectionStatus.Disconnected;
        ConnectionChanged?.Invoke(this, Status);
    }

    /// <summary>
    /// Create a new character
    /// </summary>
    /// <param name="nickname">Desired nickname</param>
    public void SendCreateChar(string nickname)
    {
        if (Status != ConnectionStatus.CorrectNumeric && Status != ConnectionStatus.NoCharacters)
        {
            throw new Exception("Você deve estar na seleção de personagens com a numerica correta.");
        }

        var firstFreeIndex = Charlist.Charnames.Select((x, index) => new
        {
            Index = index,
            Valid = x.Length == 0
        }).ToList();

        var validSlot = firstFreeIndex.FirstOrDefault(x => x.Valid);
        if (validSlot is null) throw new Exception("Sem espaço disponível.");

        var packet = P20F_CreateCharacter.Create(validSlot.Index, nickname);
        Socket.SendEncrypted(packet);
    }

    /// <summary>
    ///     Send world request with informed character index
    /// </summary>
    /// <param name="index">Character index</param>
    public void SendWorld(int index)
    {
        var packet = P213_SendWorld.Create(index);
        Socket.SendEncrypted(packet);
    }

    /// <summary>
    ///     Send signal keeping client alive
    /// </summary>
    public void SendSignal()
    {
        var packet = new P3A0_Signal();

        Socket.SendEncrypted(packet);
    }

    #endregion

    #region receive

    internal void ReceiveCharlist(P10A_CharList packet)
    {
        CommunicationControl.Keys = packet.HashKeys;
        Status = ConnectionStatus.Charlist;
        ConnectionChanged?.Invoke(this, Status);
    }

    internal void ReceiveCorrectNumeric()
    {
        if (Charlist.Charnames.Any(x => x.Length > 0))
            Status = ConnectionStatus.CorrectNumeric;
        else
        {
            Status = ConnectionStatus.NoCharacters;
        }

        ConnectionChanged?.Invoke(this, Status);
    }

    internal void ReceiveIncorrectNumeric()
    {
        Status = ConnectionStatus.WrongNumeric;
        ConnectionChanged?.Invoke(this, Status);
    }

    public void ReceiveWorld()
    {
        Status = ConnectionStatus.World;
        ConnectionChanged?.Invoke(this, Status);
        PlayerChanged?.Invoke(this, Player);
        Player.Party.Clear();

        var signalWorker = new SignalWorker(this)
        {
            Id = Guid.NewGuid().ToString()
        };
        signalWorker.Start();
    }

    internal void ReceiveDisconnect()
    {
        GuildedService.Log(GuildedLogType.Behavior, $"Conta desconectada {Player.Name}", $"Shutdown",
            DateTime.Now.ToShortDateString());
        Status = ConnectionStatus.Disconnected;
        ConnectionChanged?.Invoke(this, Status);
        Socket.Dispose();
    }

    #endregion
    private void Configure()
    {
        Socket.MessageReceived += _interpreter.Received;
    }
    
    public void Dispose()
    {
        GamePool.RemoveById(Id);
        Works.StopAll();
        Socket.MessageReceived -= _interpreter.Received;
        Socket.Dispose();
        Status = ConnectionStatus.Disconnected;
    }
}