using RuneRealm.Entities;

namespace RuneRealm.Network.Packet;

public class PacketParameters
{
    public int OpCode { get; set; }
    public int Length { get; set; }
    public Player Player { get; set; }
}