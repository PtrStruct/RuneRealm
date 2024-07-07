using RuneRealm.Commands;
using RuneRealm.Entities;

namespace RuneRealm.Factories;

public class CommandFactory
{
    public static CommandBase CreateCommand(string[] _commandArgs, Player player)
    {
        var commandName = _commandArgs[0];
        return commandName switch
        {
            "pos" => new PrintPositionCommand(player, _commandArgs),
            "anim" => new AnimationCommand(player, _commandArgs),
            "event" => new CreateEventCommand(player, _commandArgs),
            _ => new NullCommand(player, _commandArgs)
        };
    }
}