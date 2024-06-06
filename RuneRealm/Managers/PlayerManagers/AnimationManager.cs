namespace RuneRealm.Managers;

public class AnimationManager
{
    public int Stand { get; set; } = 0x328;
    public int StandTurn { get; set; } = 0x337;
    public int Walk { get; set; } = 0x333;
    public int Turn180 { get; set; } = 0x334;
    public int Turn90CW { get; set; } = 0x335;
    public int Turn90CCW { get; set; } = 0x336;
    public int Run { get; set; } = 0x338;

    public List<int> GetAnimations()
    {
        return new List<int>
        {
            Stand,
            StandTurn,
            Walk,
            Turn180,
            Turn90CW,
            Turn90CCW,
            Run
        };
    }
}