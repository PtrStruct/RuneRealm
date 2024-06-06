using RuneRealm.Environment;
using RuneRealm.Movement;

namespace RuneRealm.Entities;

public abstract class Entity
{
    public Location Location { get; set; }
    public MovementHandler MovementHandler { get; set; }

    public Entity()
    {
        MovementHandler = new MovementHandler(this);
    }
}