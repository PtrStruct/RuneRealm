using RuneRealm.Constants;
using RuneRealm.Entities;

namespace RuneRealm.Commands;

public abstract class CommandBase
{
    protected Player Player { get; set; }
    protected string[] Args { get; set; }

    public CommandBase(Player player, string[] args)
    {
        Player = player;
        Args = args;
    }
    
    public void Execute()
    {
        if (!HasRequiredRights())
        {
            Player.PacketBuilder.SendMessage($"You do not have the rights to use the {Args[0]} command.");
            return;
        }

        var validationError = ValidateArgs();
        if (!string.IsNullOrEmpty(validationError))
        {
            Player.PacketBuilder.SendMessage($"{validationError}");
            return;
        }

        try
        {
            Invoke();
        }
        catch (Exception ex)
        {
            Player.PacketBuilder.SendMessage($"Error executing command: {ex.Message}");
        }
    }
    
    protected virtual PlayerRights RequiredRights => PlayerRights.NORMAL;
    protected virtual bool HasRequiredRights() => Player.Rights >= RequiredRights;

    protected abstract string ValidateArgs();
    protected abstract void Invoke();
    
}