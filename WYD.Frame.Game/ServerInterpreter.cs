using WYD.Frame.Game.Models.Game;
using WYD.Frame.Packets;
using WYD.Frame.Packets.Network;

namespace WYD.Frame.Game;

internal class ServerInterpreter
{
    private readonly WClient _wClient;

    public ServerInterpreter(WClient wClient)
    {
        _wClient = wClient;
    }

    public void Received(object? sender, DataPayloadEventArgs args)
    {
        var buffer = args.Buffer;
        var header = IPacket.FromBytes<NetworkHeader>(buffer);

        _wClient.CommunicationControl.ServerTime = header.Timestamp;
        _wClient.CommunicationControl.TickCountServerTime = Environment.TickCount;


        switch (header.PacketId)
        {
            case 0x114:
                var receiveWorldPacket = IPacket.FromBytes<P114_SentToWorld>(buffer);
                _wClient.Player.ReceiveWorld(receiveWorldPacket);
                _wClient.Bag.ReceiveWorld(receiveWorldPacket);
                _wClient.ReceiveWorld();
                break;
            case 0x10A:
                var receiveCharListPacket = IPacket.FromBytes<P10A_CharList>(buffer);
                _wClient.ReceiveCharlist(receiveCharListPacket);
                _wClient.Charlist.ReceiveCharlist(receiveCharListPacket);
                _wClient.Bag.ReceiveCharlist(receiveCharListPacket);
                break;
            case 0x110:
                var receiveUpdateCharList = IPacket.FromBytes<P110_UpdateCharlist>(buffer);
                _wClient.Charlist.ReceiveUpdateCharlist(receiveUpdateCharList);
                break;
            case 0x2454:
                var addEvoksPacket = IPacket.FromBytes<P2454_AddEvok>(buffer);
                _wClient.Player.ReceiveAddEvoks(addEvoksPacket);
                break;
            case 0x3B9:
                var affectInfo = IPacket.FromBytes<P3B9_AffectInfo>(buffer);
                _wClient.Player.ReceiveAffectInfo(affectInfo);
                break;
            case 0x334:
                var chatPacket = IPacket.FromBytes<P334_SendChat>(buffer);
                _wClient.Social.ReceiveChat(chatPacket);
                break;
            case 0x333:
                var surroundPacket = IPacket.FromBytes<P333_SendChat>(buffer);
                _wClient.Social.ReceiveChatSurroundings(surroundPacket);
                break;
            case 0xFDE:
                _wClient.ReceiveCorrectNumeric();
                break;
            case 0xFDF:
                _wClient.ReceiveIncorrectNumeric();
                break;
            case 0x182:
                var itemPacket = IPacket.FromBytes<P182_RcvItem>(buffer);
                _wClient.Bag.ReceiveItem(itemPacket);
                break;
            case 0x3A5:
                var yellowTimePacket = IPacket.FromBytes<P3A5_YellowTime>(buffer);
                _wClient.Timer.ReceiveYellowTime(yellowTimePacket);
                break;
            case 0x101:
                var messagePacket = IPacket.FromBytes<P101_ServerMessage>(buffer);
                _wClient.Social.ReceiveMessage(messagePacket);
                break;
            case 0x338:
                var mobDeathPacket = IPacket.FromBytes<P338_MobDeath>(buffer);
                _wClient.World.ReceiveMobDeath(mobDeathPacket);
                break;
            case 0x36C:
                var mobMovePacket = IPacket.FromBytes<P36C_Move>(buffer);
                _wClient.World.ReceiveMobMove(mobMovePacket);
                break;
            case 0x366:
                var mobStopPacket = IPacket.FromBytes<P366_Stop>(buffer);
                _wClient.World.ReceiveMobStop(mobStopPacket);
                break;
            case 0x363:
                _wClient.World.ReceiveMobTrade();
                break;
            case 0x376:
                var movedItemPacket = IPacket.FromBytes<P376_MovedItem>(buffer);
                _wClient.Bag.ReceiveMovedItem(movedItemPacket);
                break;
            case 0x364:
                var newMobPacket = IPacket.FromBytes<P364_NewMob>(buffer);
                _wClient.World.ReceiveNewMob(newMobPacket);
                break;
            case 0x1C6:
                var quizPacket = IPacket.FromBytes<P1C6_Quiz>(buffer);
                _wClient.Social.ReceiveQuiz(quizPacket);
                break;
            case 0x37E:
                var removePartyPacket = IPacket.FromBytes<P37E_PartyLeft>(buffer);
                _wClient.Player.ReceiveRemoveParty(removePartyPacket);
                break;
            case 0x37D:
                var addPartyPacket = IPacket.FromBytes<P37D_PartyJoined>(buffer);
                _wClient.Player.ReceiveAddParty(addPartyPacket);
                break;
            case 0x37F:
                var partyRequestPacket = IPacket.FromBytes<P37F_PartyRequest>(buffer);
                _wClient.Player.ReceivePartyRequest(partyRequestPacket);
                break;
            case 0x165:
                var removeMobPacket = IPacket.FromBytes<P165_RemoveMob>(buffer);
                _wClient.World.ReceiveRemoveMob(removeMobPacket);
                break;
            case 0xD1D:
                var whisperPacket = IPacket.FromBytes<PD1D_ServerSMS>(buffer);
                _wClient.Social.ReceiveServerMessage(whisperPacket);
                break;
            case 0x3AF:
                var updateGoldPacket = IPacket.FromBytes<P3AF_UpdateGold>(buffer);
                _wClient.Player.ReceiveUpdateGold(updateGoldPacket);
                break;
            case 0x181:
                var updateHealthPacket = IPacket.FromBytes<P181_UpdateHpMp>(buffer);
                _wClient.Player.ReceiveUpdateHealth(updateHealthPacket);
                break;
            case 0x336:
                var updateScorePacket = IPacket.FromBytes<P336_UpdateScore>(buffer);
                _wClient.Player.ReceiveUpdateScore(updateScorePacket);
                break;
            case 0x3A1:
                var greenTimePacket = IPacket.FromBytes<P3A1_GreenTime>(buffer);
                _wClient.Timer.ReceiveGreenTime(greenTimePacket);
                break;
            case 0x11D:
                _wClient.ReceiveDisconnect();
                break;
            case 0x3A8:
                Console.WriteLine("Sent war info.");
                break;
            case 0x337:
                var updateEtcData = IPacket.FromBytes<P337_UpdateScoreEtc>(buffer);
                _wClient.Player.ReceiveUpdateEtc(updateEtcData);
                break;
            case 0x18B:
                Console.WriteLine("Sent update weather.");
                break;
            case 0x36A: //Fogos de artificio
                break;
            case 0x39D: //Attack one
                break;
            case 0x18A: //Hp Dam
                break;
            case 0x36B: //Update equips
                break;
            case 0x17C:
                var shopListPacketData = IPacket.FromBytes<P17C_ShopList>(buffer);
                _wClient.World.ReceiveShopList(shopListPacketData);
                break;
            case 0x379:
                var buyedItemData = IPacket.FromBytes<P379_BuyNpcItem>(buffer);
                _wClient.Bag.ReceiveItemBuy(buyedItemData);
                break;
            case 0x185:
                var updateInventoryData = IPacket.FromBytes<P185_UpdateInventory>(buffer);
                _wClient.Bag.ReceiveUpdateInventory(updateInventoryData);
                break;
            case 0x383:
                var tradePacketData = IPacket.FromBytes<P383_Trade>(buffer);
                _wClient.Bag.ReceivedTrade(trade: tradePacketData);
                break;
            default:
                //Console.WriteLine($"Unknown packet: {header.PacketId.ToString("X2")} Data [{Helpers.StringHelper.ByteArrayToHexString(args.EncBuffer)}]");
                break;
        }
    }
}