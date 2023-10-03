namespace WYD.Frame.Models.Models;

public class CommunicationControl
{
    /// <summary>
    /// Atualizado sempre que o cliente recebe um pacote do servidor. Essa variavel é setada para o Timestamp do header do pacote.
    /// </summary>
    public uint ServerTime { get; set; }
    
    /// <summary>
    /// Tick count atual da maquina quando o cliente recebe um pacote do servidor.
    /// </summary>
    public int TickCountServerTime { get; set; }
    
    /// <summary>
    /// Essa aqui é a diferença de tempo entre o ultimo timestamp do servidor e o ticket count atual da maquina
    /// </summary>
    public long CurrentTime => ServerTime + (Environment.TickCount - TickCountServerTime);
    public int CurrentKeyIndex { get; set; } = default;
    public byte[]? Keys { get; set; } = null;
}