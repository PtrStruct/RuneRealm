using RuneRealm.Constants;

namespace RuneRealm.Network.Packet;

public class PacketStore
{
    private readonly Dictionary<ClientOpCodes, int> _maxPacketCounts = new()
    {
        { ClientOpCodes.ItemAction1, 10 }, /* OpCode, How many to allow to be processed per tick */
        { ClientOpCodes.Chat, 10 },
        { ClientOpCodes.MouseClick, 10 },
        { ClientOpCodes.CameraMovement, 10 },
        { ClientOpCodes.FocusChange, 10 },
        { ClientOpCodes.EquipItem, 10 },

        /* Movement */
        { ClientOpCodes.WalkOnCommand, 10 },
        { ClientOpCodes.RegularWalk, 10 },
        { ClientOpCodes.MapWalk, 10 },

        { ClientOpCodes.PlayerCommand, 10 },
        { ClientOpCodes.AttackNpc, 10 },
        { ClientOpCodes.MoveItem, 10 },
        { ClientOpCodes.UnequipItem, 10 },
        { ClientOpCodes.ButtonClick, 10 },
        { ClientOpCodes.PickupGroundItem, 10 },
        { ClientOpCodes.Dialogue, 10 },
        { ClientOpCodes.NpcAction1, 10 }
    };

    public Dictionary<ClientOpCodes, List<IPacket>> Packets = new();

    public void AddPacket(ClientOpCodes opCode, IPacket packet)
    {
        if (packet == null) return;

        if (!Packets.ContainsKey(opCode)) Packets[opCode] = new List<IPacket>();

        // Check count of added packets against the max allowed for this opCode
        var currentPacketCount = Packets[opCode].Count;
        if (!_maxPacketCounts.TryGetValue(opCode, out var maxPacketCount))
            // Handle the case when there is no defined maxPacketCount for this opCode.
            // For example, set a default value or ignore the packet.
            maxPacketCount = 1; // replace with a defined constant or a chosen value

        if (currentPacketCount < maxPacketCount)
        {
            Packets[opCode].Add(packet);
        }
        // Handle case when the max packet count for this opCode is reached.
        // For example, discard the packet or put it in a queue for later processing.
    }

    public void ProcessPackets()
    {
        foreach (var packetList in Packets.Values)
        foreach (var packet in packetList.ToList())
            if (packetList.Remove(packet))
                packet.Process();
    }
}