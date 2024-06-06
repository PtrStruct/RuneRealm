using RuneRealm.Constants;

namespace RuneRealm.Managers;

public class ColorManager
{
    public HairColor Hair { get; set; } = HairColor.White;
    public TorsoColor Torso { get; set; } = TorsoColor.Black;
    public LegColor Legs { get; set; } = LegColor.Black;
    public FeetColor Feet { get; set; } = FeetColor.Brown;
    public SkinColor Skin { get; set; } = SkinColor.NormalWhite;

    public List<int> GetColors()
    {
        return new List<int>
        {
            (int)Hair,
            (int)Torso,
            (int)Legs,
            (int)Feet,
            (int)Skin
        };
    }
}