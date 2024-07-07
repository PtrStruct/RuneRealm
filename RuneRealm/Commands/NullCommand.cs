using RuneRealm.Constants;
using RuneRealm.Entities;

namespace RuneRealm.Commands;

public class NullCommand : CommandBase
{
    private readonly string _commandName;

    public NullCommand(Player player, string[] args) : base(player, args)
    {
        _commandName = args[0];
    }
    
    protected override PlayerRights RequiredRights => PlayerRights.NORMAL;
    
    protected override string ValidateArgs()
    {
        return null;
    }

    protected override void Invoke()
    {
        Player.PacketBuilder.SendMessage($"Command {_commandName} not recognized.");
    }
}