using RuneRealm.Entities;
using RuneRealm.Factories;

namespace RuneRealm.Network.Packets.Incoming;

public class PlayerCommandPacket : IPacket
{
    private readonly string[] _commandArgs;
    private readonly int _length;
    private readonly int _opcode;
    private readonly Player _player;

    public PlayerCommandPacket(dynamic parameters)
    {
        _player = parameters.Player;
        _opcode = parameters.OpCode;
        _length = parameters.Length;
        _commandArgs = _player.Session.Reader.ReadString().Split(' ');
    }

    public void Process()
    {
        var command = CommandFactory.CreateCommand(_commandArgs, _player);
        command.Execute();
    }
}