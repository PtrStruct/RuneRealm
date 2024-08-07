﻿using RuneRealm.Constants;
using RuneRealm.Network.Packets.Incoming;

namespace RuneRealm.Network.Packets;

public static class PacketFactory
{
    public static IPacket CreateClientPacket(ClientOpCodes opcode, dynamic parameters)
    {
        switch (opcode)
        {
            case ClientOpCodes.RegularWalk:
            case ClientOpCodes.MapWalk:
            case ClientOpCodes.WalkOnCommand:
                return new WalkPacket(parameters);
            case ClientOpCodes.ObjectAction1:
                return new WorldObjectInteractionFirstClick(parameters);
            // case 122:
            //     return new FirstOptionClickPacket(parameters);
            case ClientOpCodes.PlayerCommand:
                return new PlayerCommandPacket(parameters);
            //case 72:
            //case 131:
            //    return new NPCInteractPacket(parameters);
            // case 40:
            //     return new ClickDialogue(parameters);
            //case 41:
            //    return new EquipItemPacket(parameters);
            //case 214:
            //    return new MoveItemPacket(parameters);
            //case 145:
            //    return new RemoveItemPacket(parameters);
            //case 185:
            //    return new ActionButtonPacket(parameters);
            //case 53:
            //    return new ItemOnItemPacket(parameters);
            // case 236:
            //     return new PickupItem(parameters);
            // case 87:
            //     return new DropItem(parameters);
            // case 40:
            //     return new DialoguePacket(parameters);
            // case 155:
            //     return new FirstNPCActionPacket(parameters);
            default:
                Console.WriteLine($"No packet class implementation for opcode {opcode}.");
                return null;
        }
    }
}