using WYD.Frame.Game;
using WYD.Frame.Models.Models;
using WYD.Frame.Services;
using WYD.Frame.Services.Common;

namespace WYD.Frame.Tests;

public class MoveTest
{
    [SetUp]
    public void Setup()
    {
        var wydClienteConfig = new ClientConfiguration()
        {
            Credentials = new ClientCredentials()
            {
                Numeric = "1213",
                Password = "123",
                Username = "123"
            },
            ConnectionConfiguration = new()
            {
                ServerIp = "192.168.2.1",
                ServerClientVersion = 0x7556,
                ServerPort = 8281
            },
            GeneralConfig = new GeneralConfig()
            {
                Behavior = new BehaviorConfig()
                {
                    NotifyGuilded = false,
                    ReviveRandom = true,
                    ReviveAfterSeconds = 1000,
                    TurnoffWorkersOnDeath = true
                },
                Id = Guid.NewGuid().ToString()
            },
            HwidInfo = new HwidInfo()
            {
                HardDisk = "",
                MoboManufacturer = "MSI",
                MoboName = "B550"
            },
            QuizConfiguration = new QuizConfiguration()
            {
                QuestionResponses = new List<QuizQuestionResponse>()
                {
                    new QuizQuestionResponse()
                    {
                        Question = "Que nivel começa?",
                        Answer = "1"
                    }
                }
            }
        };
        
        var wydCliente = WClient.Build(wydClienteConfig);
    }

    [Test]
    public void Path_Should_Be_Found()
    {
        var area = LocationService.Areas.First(x => x.Name == AreaNames.Armia);

        var finderForNode = MapService.CreateForNode(area);
        var origin = area.ReduceToArea(new Position(2088, 2106));

        var destination = area.ReduceToArea(new Position(2140, 2087));

        var positionsToWalk =
            PathingService.FindPath(finderForNode, origin, destination);

        foreach (var path in positionsToWalk)
        {
            Console.WriteLine($"Walk to {area.Bounds.Min.X + path.X} / {area.Bounds.Min.Y + path.Y}");
        }

        Assert.Greater(positionsToWalk.Length, 1);
    }
}