using System.Net.Sockets;
using System.Runtime.InteropServices;
using Starksoft.Net.Proxy;
using WYD.Frame.Game.Helpers;
using WYD.Frame.Game.Models;
using WYD.Frame.Game.Models.Game;
using WYD.Frame.Game.Security;
using WYD.Frame.Packets;
using WYD.Frame.Packets.Network;

namespace WYD.Frame.Game;

internal class ClientSocket
{
    private readonly BufferPool _bufferPool = new();
    private readonly WClient _wClient;
    private TcpClient _socketClient;

    public ClientSocket(WClient wClient)
    {
        _wClient = wClient;
        _socketClient = new TcpClient(AddressFamily.InterNetwork);
    }

    public event EventHandler<DataPayloadEventArgs>? MessageReceived;

    public async Task ConnectAsync()
    {
        try
        {
            if (_wClient.Configuration.ProxyConfiguration.UseProxy)
            {
                var proxyConfig = _wClient.Configuration.ProxyConfiguration;
                var proxyClient = new Socks5ProxyClient(proxyConfig.Host, proxyConfig.Port, proxyConfig.UserName,
                    proxyConfig.Password);
                _socketClient = proxyClient.CreateConnection(_wClient.Configuration.ConnectionConfiguration.ServerIp,
                    _wClient.Configuration.ConnectionConfiguration.ServerPort);
            }
            else
            {
                _socketClient = new TcpClient(AddressFamily.InterNetwork);
                await _socketClient.ConnectAsync(_wClient.Configuration.ConnectionConfiguration.ServerIp,
                    _wClient.Configuration.ConnectionConfiguration.ServerPort);
            }

            var thread = new Thread(EstablishReceiver);
            thread.Start();
        }
        catch (Exception ex)
        {
            throw new Exception($"Conexão falhou. {ex.Message}");
        }
       
    }

    private void EstablishReceiver()
    {
        try
        {
            var state = new StateObject(_socketClient.Client);
            _socketClient.Client.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, DataReceived,
                state);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Establish Error: " + ex.Message);
        }
    }

    private void DataReceived(IAsyncResult result)
    {
        try
        {
            var state = (StateObject)result.AsyncState!;

            var bytesCount = 0;

            SocketError errorCode;
            bytesCount = state.Socket.EndReceive(result, out errorCode);
            if (errorCode != SocketError.Success) bytesCount = 0;

            if (bytesCount > 0)
            {
                _bufferPool.Receive(state.Buffer.Take(bytesCount).ToArray());

                while (_bufferPool.HasData())
                {
                    var handler = MessageReceived;
                    var buffer = _bufferPool.ReadNext();
                    var encBuffer = new byte[buffer.Length];
                    
                    buffer.CopyTo(encBuffer, 0);
                    GameSecurity.Decrypt(buffer);
                    handler?.Invoke(this, new DataPayloadEventArgs()
                    {
                        Buffer = buffer,
                        EncBuffer = encBuffer
                    });
                }

                EstablishReceiver();
            }
            else
            {
                _wClient.ReceiveDisconnect();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"WEE: {e.Message} / {e.StackTrace}");
            _bufferPool.Clear();
            _wClient.ReceiveDisconnect();
        }
    }

    public void Dispose()
    {
        try
        {
            _socketClient.Client?.Disconnect(false);
            _socketClient.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            //ignore
        }

        _socketClient = new TcpClient();
    }

    public void SendEncryptedNumeric(PFDE_Numeric numeric)
    {
        var packetHeader = new NetworkHeader
        {
            Key = GameSecurity.NextKey(_wClient.CommunicationControl.Keys, _wClient.CommunicationControl.CurrentKeyIndex, true),
            Checksum = 0
        };
        for(int i =0; i < numeric.Numeric.Length; i++)
        {
            numeric.Numeric[i] ^= packetHeader.Key;
        }
      

        if (_wClient.CommunicationControl is { Keys: { }, CurrentKeyIndex: < 16 })
            _wClient.CommunicationControl.CurrentKeyIndex++;

        var attribute = typeof(PFDE_Numeric).GetCustomAttributes(typeof(Packet), false).FirstOrDefault() as Packet;
        if (attribute is null)
            throw new Exception($"Todo pacote deve ter o atributo Packet informando seu Id. {typeof(PFDE_Numeric)}");

        packetHeader.Size = (ushort)Marshal.SizeOf<PFDE_Numeric>();
        packetHeader.PacketId = attribute.Id;
        packetHeader.ClientId = _wClient.Player.ClientId;
        packetHeader.Timestamp = (uint)_wClient.CommunicationControl.CurrentTime;


        numeric.Header = packetHeader;

        var buffer = IPacket.ToBytes(numeric);
        GameSecurity.Encrypt(ref buffer);
        Send(buffer);
    }

    public NetworkHeader GetBasicHeader<T>(T packet) where T : IPacket
    {
        var packetHeader = new NetworkHeader
        {
            Key = GameSecurity.NextKey(_wClient.CommunicationControl.Keys, _wClient.CommunicationControl.CurrentKeyIndex, true),
            Checksum = 0
        };

        if (_wClient.CommunicationControl is { Keys: { }, CurrentKeyIndex: < 16 })
            _wClient.CommunicationControl.CurrentKeyIndex++;
        var attribute = typeof(T).GetCustomAttributes(typeof(Packet), false).FirstOrDefault() as Packet;
        if (attribute is null)
            throw new Exception($"Todo pacote deve ter o atributo Packet informando seu Id. {typeof(T)}");

        packetHeader.PacketId = attribute.Id;
        packetHeader.ClientId = _wClient.Player.ClientId;
        packetHeader.Timestamp = (uint)_wClient.CommunicationControl.CurrentTime;

        return packetHeader;
    }
    public void SendEncrypted<T>(T packet) where T : IPacket
    {
        var packetHeader = new NetworkHeader
        {
            Key = GameSecurity.NextKey(_wClient.CommunicationControl.Keys, _wClient.CommunicationControl.CurrentKeyIndex, true),
            Checksum = 0
        };

        if (_wClient.CommunicationControl is { Keys: { }, CurrentKeyIndex: < 16 })
            _wClient.CommunicationControl.CurrentKeyIndex++;

        var attribute = typeof(T).GetCustomAttributes(typeof(Packet), false).FirstOrDefault() as Packet;
        if (attribute is null)
            throw new Exception($"Todo pacote deve ter o atributo Packet informando seu Id. {typeof(T)}");

        packetHeader.Size = (ushort)Marshal.SizeOf<T>();
        packetHeader.PacketId = attribute.Id;
        packetHeader.ClientId = _wClient.Player.ClientId;
        packetHeader.Timestamp = (uint)_wClient.CommunicationControl.CurrentTime;


        packet.Header = packetHeader;
        

        var buffer = IPacket.ToBytes(packet);
        // Console.WriteLine($"SND: {packet.Header.PacketId.ToString("X2")}, {StringHelper.ByteArrayToHexString(buffer)}");
        GameSecurity.Encrypt(ref buffer);
        Send(buffer);
    }

    public void Send(byte[] buffer)
    {
        try
        {
            _socketClient.Client.Send(buffer);
        }
        catch (Exception)
        {
            Console.WriteLine("Send error");
        }
    }

    private class StateObject
    {
        public int ReadOffset = 0;

        public StateObject(Socket socket)
        {
            Socket = socket;
        }

        public Socket Socket { get; }

        public byte[] Buffer { get; } = new byte[1000];
    }

    private class BufferPool
    {
        private readonly List<byte> _pendingBuffer = new();

        public bool HasData()
        {
            if (_pendingBuffer.Count < 2) return false;

            var size = BitConverter.ToInt16(_pendingBuffer.Take(2).ToArray(), 0);

            return size <= _pendingBuffer.Count && size > 0;
        }

        public byte[] ReadNext()
        {
            var size = BitConverter.ToInt16(_pendingBuffer.Take(2).ToArray(), 0);
            
            var takenData = _pendingBuffer.Take(size).ToArray();
            _pendingBuffer.RemoveRange(0, size);

            return takenData;
        }

        public void Receive(byte[] buffer)
        {
            _pendingBuffer.AddRange(buffer);
        }

        public void Clear()
        {
            _pendingBuffer.Clear();
        }
    }
}