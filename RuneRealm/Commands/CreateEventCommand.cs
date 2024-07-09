using RuneRealm.Entities;
using RuneRealm.Events;

namespace RuneRealm.Commands;

public class CreateEventCommand : CommandBase
{
    public CreateEventCommand(Player player, string[] args) : base(player, args)
    {
    }

    protected override string ValidateArgs()
    {
        return null;
    }

    protected override void Invoke()
    {
        // Player.SetCurrentAnimation(422);
        var guid = Player.StartNewTask();
        RSEventHandler.Instance.AddEvent(Player, new CombatEvent(Player, guid), 1);
    }
}