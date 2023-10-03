using System.Text;
using System.Text.RegularExpressions;
using F23.StringSimilarity;
using WYD.Frame.Common;
using WYD.Frame.Common.Enum;
using WYD.Frame.Common.Enum.Game;
using WYD.Frame.Game.Helpers;
using WYD.Frame.Game.Models;
using WYD.Frame.Game.Models.Game;
using WYD.Frame.Packets;
using WYD.Frame.Packets.Network;
using WYD.Frame.Services;

namespace WYD.Frame.Game;

public class Social
{
    private readonly WClient _wClient;

    public EventHandler<GameMessage>? MessageReceived;
    public EventHandler<string>? UnknownQuizQuestionReceived;

    public Social(WClient wClient)
    {
        _wClient = wClient;
    }

    public Queue<GameMessage> Messages { get; } = new();


    /// <summary>
    ///     Send chat
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="command">Command, it can be player or a game command, eg: /kingdom</param>
    public void SendChat(string message, string command)
    {
        var packet = P334_SendChat.Create(message, command);

        _wClient.Socket.SendEncrypted(packet);
    }

    /// <summary>
    ///     Send surrounding chat
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="command">Command, it can be player or a game command, eg: /kingdom</param>
    public void SendChatSurrounds(string message)
    {
        var packet = P333_SendChat.Create(message);

        _wClient.Socket.SendEncrypted(packet);
    }

    /// <summary>
    ///     Answer a game quiz
    /// </summary>
    /// <param name="responseIndex">Response index (usually from 0 to 3)</param>
    public void SendQuizResponse(int responseIndex)
    {
        var packet = P2C7_SendQuizResponse.Create(responseIndex);
        _wClient.Socket.SendEncrypted(packet);
    }

    internal void ReceiveQuiz(P1C6_Quiz packetData)
    {
        

        var title = Encoding.Latin1.GetString(packetData.Title.TrimTailingZeros());
        _wClient.Log(MessageRelevance.Highest, $"[Quiz] Server asks: ({title})");

        var options = packetData.Options.Select(x => Encoding.Latin1.GetString(x.Text.TrimTailingZeros())).ToList();
        if (title.StartsWith("Quanto é"))
        {

            var operation = title.FirstOrDefault(x => x is '-' or 'x' or '+');
            var split = title.Split(operation);
            var numbers = split.Select(x => Regex.Match(x, @"\d+").Value).ToList();
            var response = operation.Operate(numbers[0], numbers[1]);
            _wClient.Log(MessageRelevance.Highest, $"[Quiz] You answer ({response})");

            var result = options.ToList().FindIndex(x => int.Parse(x.Split(')').Last()) == response);

            SendQuizResponse(result);
        }
        else
        {
            var jaroAlgoritmh = new JaroWinkler();

            var configs = _wClient.Configuration.QuizConfiguration.QuestionResponses.Select(x => new
            {
                Similarity = jaroAlgoritmh.Similarity(title, x.Question),
                Question = x.Question,
                Answer = x.Answer
            }).MaxBy(x => x.Similarity);

            if (configs is null || configs.Similarity <= 0.9)
            {
                UnknownQuizQuestionReceived?.Invoke(_wClient, title);
                return;
            }

            if (configs.Answer is null)
            {
                UnknownQuizQuestionReceived?.Invoke(_wClient, title);
                return;
            }
            var answer = options.ToList().Select(x => new
            {
                Similarity = jaroAlgoritmh.Similarity(x, configs.Answer),
                Answer = x
            }).MaxBy(x => x.Similarity);

            if (answer is null || answer.Similarity <= 0.8)
            {
                UnknownQuizQuestionReceived?.Invoke(_wClient, title);
                return;
            }

            var result = options.ToList().FindIndex(x => x.Equals(answer.Answer));
            _wClient.Log(MessageRelevance.Highest, $"[Quiz] You answer ({answer.Answer}) - index {result}");

            SendQuizResponse(result);
        }
    }

    internal void ReceiveServerMessage(PD1D_ServerSMS packetData)
    {
        var messageTuple = new GameMessage()
        {
            Command = packetData.Nickname,
            Message = packetData.Message,
            Relevance = MessageRelevance.Highest,
            MessageOrigin = MessageOrigin.Server
        };
        Messages.Enqueue(messageTuple);

        if (Messages.Count == 100) Messages.Dequeue();
        

        MessageReceived?.Invoke(_wClient, messageTuple);
    }

    internal void ReceiveMessage(P101_ServerMessage p101ServerMessage)
    {
        var messageTuple = new GameMessage()
        {
            Command = "Server",
            Message = p101ServerMessage.Message,
            Relevance = MessageRelevance.Highest,
            MessageOrigin = MessageOrigin.Server
        };
        Messages.Enqueue(messageTuple);

        if (Messages.Count == 100) Messages.Dequeue();

        MessageReceived?.Invoke(_wClient, messageTuple);

        if (p101ServerMessage.Message.StartsWith("Ocorreu um erro, contate a administra"))
        {
            _wClient.SendDisconnect();
        }
    }

    internal void ReceiveChatSurroundings(P333_SendChat packetData)
    {
        var gameMessage = new GameMessage()
        {
            Command = "Surroundings",
            Message = packetData.Message,
            Relevance = MessageRelevance.Low,
            MessageOrigin = MessageOrigin.Chat
        };
        gameMessage.MessageOrigin = MessageOrigin.Whisper;

        Messages.Enqueue(gameMessage);

        if (Messages.Count == 100) Messages.Dequeue();

        MessageReceived?.Invoke(_wClient, gameMessage);
    }

    internal void ReceiveChat(P334_SendChat packetData)
    {
        var gameMessage = new GameMessage()
        {
            Command = packetData.Command,
            Message = packetData.Message,
            Relevance = MessageRelevance.Highest,
            MessageOrigin = MessageOrigin.Chat
        };
        if (packetData.Message.StartsWith("@@") || packetData.Message.StartsWith("@"))
        {
            gameMessage.Relevance = MessageRelevance.Low;
            Messages.Enqueue(gameMessage);

            if (Messages.Count == 100) Messages.Dequeue();

            MessageReceived?.Invoke(_wClient, gameMessage);
        }
        else
        {
            gameMessage.MessageOrigin = MessageOrigin.Whisper;

            Messages.Enqueue(gameMessage);

            if (Messages.Count == 100) Messages.Dequeue();

            MessageReceived?.Invoke(_wClient, gameMessage);
            GuildedService.Log(GuildedLogType.Whisper, packetData.Message, packetData.Command, $"Bot player: " + _wClient.Player.Name);
        }
    }
}