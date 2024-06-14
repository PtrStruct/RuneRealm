using System.Numerics;
using RuneRealm.Environment;
using RuneRealm.Movement;
using RuneRealm.Tasks;

namespace RuneRealm.Entities;

public class NPC : Entity
{
    public override Location Location { get; set; }
    public override MovementHandler MovementHandler { get; set; }
    public override RSTaskScheduler TaskScheduler { get; set; } = new();

    public override void SetInteractingEntity(Entity entity)
    {
        throw new NotImplementedException();
    }

    public override void SetFacingDirection(Vector2 vector)
    {
        throw new NotImplementedException();
    }

    public override void SetCurrentAnimation(int animationId)
    {
        throw new NotImplementedException();
    }

    public override void SetCurrentGfx(int gfx)
    {
        throw new NotImplementedException();
    }

    public override void Reset()
    {
        throw new NotImplementedException();
    }
}