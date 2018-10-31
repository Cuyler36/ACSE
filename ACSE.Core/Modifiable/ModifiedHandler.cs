using System.Collections.Generic;
using System.Linq;

namespace ACSE.Core.Modifiable
{
    public class ModifiedHandler : IModifiable
    {
        public readonly Stack<IModifiable> UndoStack;
        public readonly Stack<IModifiable> RedoStack;

        public ModifiedHandler()
        {
            UndoStack = new Stack<IModifiable>();
            RedoStack = new Stack<IModifiable>();
        }

        // Undo & Redo logic
        public void Undo()
        {
            if (UndoStack.Any()) return;
            var previousChange = UndoStack.Pop();
            RedoStack.Push(previousChange);
            previousChange.Undo();
        }

        public void Redo()
        {
            if (!RedoStack.Any()) return;
            var previousChange = RedoStack.Pop();
            UndoStack.Push(previousChange);
            previousChange.Redo();
        }

        public void NewChange(object change)
        {
            RedoStack.Clear();
            if (!(change is IModifiable modifier)) return;
            UndoStack.Push(modifier);
        }
    }
}
