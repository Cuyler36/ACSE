namespace ACSE.Core.Modifiable
{
    /// <summary>
    /// Interface for all modifiable things that implement a modified stack.
    /// </summary>
    public interface IModifiable
    {
        void Undo();
        void Redo();
        void NewChange(object modifier);
    }
}
