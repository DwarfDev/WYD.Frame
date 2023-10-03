# WYD Framework

## Visão Geral
O WYD Framework em C# é uma poderosa biblioteca que simplifica a comunicação com um servidor do jogo chamado WYD (With Your Destiny). Este framework é construído em C# e utiliza o .NET 7. Ele oferece uma variedade de funcionalidades que permitem a criação de bots e aplicativos para interagir com o jogo de forma automatizada.

## Funcionalidades Principais
O WYD Framework oferece uma série de funcionalidades essenciais para interagir com o servidor do jogo WYD. Algumas das principais funcionalidades incluem:

Conexão com o servidor do jogo.
Movimentação do personagem dentro do mundo do jogo.
Utilização do chat para comunicação com outros jogadores.
Manipulação de eventos importantes do objeto WClient, incluindo:
PlayerChanged: Evento disparado quando as informações do jogador são alteradas.
ScoreUpdated: Evento disparado quando a pontuação do jogador é atualizada.
PartyJoined: Evento disparado quando o jogador se junta a uma festa.
PartyReceived: Evento disparado quando o jogador recebe um convite para uma festa.
PartyLeft: Evento disparado quando o jogador deixa uma festa.
LogMessageReceived: Evento disparado quando uma mensagem de log é recebida.
ConnectionChanged: Evento disparado quando o estado da conexão é alterado.

## Como Começar
Para criar uma instância do WYD Framework, siga os passos abaixo:

Inclua a DLL ou o projeto do WYD Framework em seu projeto C#.
Utilize o código a seguir para criar uma nova instância do WClient:
```csharp

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
```
Agora você está pronto para começar a usar o WYD Framework para interagir com o servidor do jogo WYD de forma automatizada.

## Requisitos
Certifique-se de que seu projeto atenda aos seguintes requisitos:

Plataforma .NET 7.
Referência à DLL ou ao projeto do WYD Framework.
Contribuição
Este projeto é de código aberto e aceita contribuições da comunidade. Se você deseja contribuir ou relatar problemas, sinta-se à vontade para fazê-lo no repositório do projeto.

## Licença
Este projeto é distribuído sob a licença MIT. Consulte o arquivo LICENSE para obter detalhes sobre os termos de uso.

Este é um projeto em andamento e estamos constantemente trabalhando para melhorá-lo. Se você tiver alguma dúvida, sugestão ou problema, entre em contato conosco. Obrigado por escolher o WYD Framework em C#!
