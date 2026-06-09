namespace TownOfUsEdited.Interfaces
{
    public interface IVisualAlteration
    {
        bool TryGetModifiedAppearance(out VisualAppearance appearance);
    }
}