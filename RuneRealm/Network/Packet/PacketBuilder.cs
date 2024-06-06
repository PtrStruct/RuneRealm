using RuneRealm.Constants;
using RuneRealm.Entities;

namespace RuneRealm.Network.Packet;

public class PacketBuilder
{
    private readonly Player _player;

    public PacketBuilder(Player player)
    {
        _player = player;
    }
    
    public void SendPlayerStatus()
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.PLAYER_STATUS);
        _player.Session.Writer.WriteByteA(0);
        _player.Session.Writer.WriteWordBigEndianA(_player.Session.Index);
    }

    public void BuildNewBuildAreaPacket()
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.REGION_LOAD);
        _player.Session.Writer.WriteWordA(_player.Location.CenterChunkX);
        _player.Session.Writer.WriteWord(_player.Location.CenterChunkY);
    }

    public void SendMessage(string message)
    {
        _player.Session.Writer.CreateFrameVarSize(ServerOpCodes.MSG_SEND);
        _player.Session.Writer.WriteString($"{message}");
        _player.Session.Writer.EndFrameVarSize();
    }
    
    public void SendSidebarInterface(int _tabId, int _displayId)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.SIDEBAR_INTF_ASSIGN);
        _player.Session.Writer.WriteWord(_displayId); /* What to display inside that tab */
        _player.Session.Writer.WriteByteA(_tabId); /* Which tab */
    }
    
    public void SendConfig(int _id, int _state)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.CONFIG_SET);
        _player.Session.Writer.WriteWordBigEndian(_id);
        _player.Session.Writer.WriteByte(_state);
    }
    
    public void UpdateSlot(int slot, int itemId, int amount, int frameId)
    {
        _player.Session.Writer.CreateFrameVarSizeWord(ServerOpCodes.ITEM_SLOT_SET);
        _player.Session.Writer.WriteWord(frameId);
        _player.Session.Writer.WriteByte(slot);
        _player.Session.Writer.WriteWord(itemId + 1);
        if (amount > 254)
        {
            _player.Session.Writer.WriteByte(255);
            _player.Session.Writer.WriteDWord(amount);
        }
        else
        {
            _player.Session.Writer.WriteByte(amount);
        }

        _player.Session.Writer.EndFrameVarSizeWord();
    }
    
    public void SendTextToInterface(string _text, int _interfaceId)
    {
        _player.Session.Writer.CreateFrameVarSizeWord(ServerOpCodes.INTF_TEXT_ADD);
        _player.Session.Writer.WriteString(_text);
        _player.Session.Writer.WriteWordA(_interfaceId);
        _player.Session.Writer.EndFrameVarSizeWord();
    }

    public void ClearInterfaces()
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.INTF_CLEAR);
    }
    
    public void SendChatInterface(int _interfaceId)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.INTF_CHAT_ADD);
        _player.Session.Writer.WriteWordBigEndian_dup(_interfaceId);
    }
    
    public void DisplayHiddenInterface(int _mainFrame, int _frameId)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.INTF_HIDDEN);
        _player.Session.Writer.WriteByte(_mainFrame); /* What to display inside that tab */
        _player.Session.Writer.WriteWord(_frameId); /* Which tab */
    }
    
    public void SendItemToInterface(int itemId, int zoom, int _interfaceId)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.INTF_MODEL_ZOOM);
        _player.Session.Writer.WriteWordBigEndian(_interfaceId);
        _player.Session.Writer.WriteWord(zoom);
        _player.Session.Writer.WriteWord(itemId);
    }
    
    public void SendInterfaceOffset(int a, int b, int barId)
    {
        _player.Session.Writer.CreateFrame(ServerOpCodes.INTF_OFFSET);
        _player.Session.Writer.WriteWord(a);
        _player.Session.Writer.WriteWordBigEndian(b);
        _player.Session.Writer.WriteWordBigEndian(barId);
    }
}