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
        Player.SetCurrentAnimation(875);
        var guid = Player.StartNewTask();
        RSEventHandler.Instance.AddEvent(Player, new MessageEvent(Player, guid), 4);
    }
}