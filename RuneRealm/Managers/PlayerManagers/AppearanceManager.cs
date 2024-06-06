using RuneRealm.Constants;

namespace RuneRealm.Managers;

public class AppearanceManager
{
    public TorsoType Torso { get; set; } = TorsoType.RegularShirt;
    public ArmType Arms { get; set; } = ArmType.ShortSleeveLessMuscles;
    public LegType Legs { get; set; } = LegType.RegularTightPants;
    public HandType Hands { get; set; } = HandType.NoCuffs;
    public FeetType Feet { get; set; } = FeetType.SmallerFeet;
    public HairType Hair { get; set; } = HairType.Spiky;
    public BeardType Beard { get; set; } = BeardType.LongBeard;
}