using RuneRealm.Environment;
using RuneRealm.Movement;
using RuneRealm.Tasks;

namespace RuneRealm.Entities;

public class NPC : Entity
{
    public override Location Location { get; set; }
    public override MovementHandler MovementHandler { get; set; }
    public override RSTaskScheduler TaskScheduler { get; set; } = new();

    public override void Reset()
    {
        throw new NotImplementedException();
    }
}