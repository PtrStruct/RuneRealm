using RuneRealm.Environment;
using RuneRealm.Movement;
using RuneRealm.Tasks;

namespace RuneRealm.Entities;

public abstract class Entity
{
    public abstract Location Location { get; set; }
    public abstract MovementHandler MovementHandler { get; set; }
    public abstract RSTaskScheduler TaskScheduler { get; set; }
    public abstract void Reset();
}