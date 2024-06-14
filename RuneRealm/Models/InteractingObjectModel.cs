using RuneRealm.Interactions;

namespace RuneRealm.Models;

public class InteractingObjectModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int ObjectData { get; set; }
    public InteractionType InteractionType { get; set; }
}