namespace ACSE
{
    /// <summary>
    /// Interface for all modifiable things that implement a modified stack.
    /// </summary>
    internal interface IModifiable
    {
        void Undo();
        void Redo();
        void NewChange(object modifier);
    }
}
