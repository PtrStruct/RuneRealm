using RuneRealm.Constants;
using RuneRealm.Entities;

namespace RuneRealm.Commands;

public class AnimationCommand : CommandBase
{
    private int _animationId;

    public AnimationCommand(Player player, string[] args) : base(player, args)
    {
        
    }

    protected override PlayerRights RequiredRights => PlayerRights.NORMAL;

    protected override string ValidateArgs()
    {
        if (Args.Length < 2)
        {
            return "Invalid syntax! Try ::anim 713";
        }

        if (!int.TryParse(Args[1], out _animationId))
        {
            return "Invalid animation ID! Try ::anim 713";
        }

        return null;
    }

    protected override void Invoke()
    {
        Player.SetCurrentAnimation(_animationId);
    }
}