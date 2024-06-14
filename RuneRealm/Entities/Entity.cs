using System.Numerics;
using RuneRealm.Data.ObjectsDef;
using RuneRealm.Environment;
using RuneRealm.Movement;
using RuneRealm.Tasks;

namespace RuneRealm.Entities;

public abstract class Entity
{
    public abstract Location Location { get; set; }
    public abstract MovementHandler MovementHandler { get; set; }
    public abstract RSTaskScheduler TaskScheduler { get; set; }

    public int CurrentAnimation { get; set; }
    public int CurrentGfx { get; set; }
    public Vector2 FacingDirection { get; set; }
    
    public abstract void SetInteractingEntity(Entity entity);
    public abstract void SetFacingDirection(Vector2 vector);
    public abstract void SetCurrentAnimation(int animationId);
    public abstract void SetCurrentGfx(int gfx);
    
    public abstract void Reset();

    
    
}