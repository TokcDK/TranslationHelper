using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Functions.UndoRedo
{
    internal class IUndoRedoCommandManager
    {
        private readonly Stack<IUndoRedoCommand> _undoStack = new Stack<IUndoRedoCommand>();
        private readonly Stack<IUndoRedoCommand> _redoStack = new Stack<IUndoRedoCommand>();

        public void Execute(IUndoRedoCommand IUndoRedoCommand)
        {
            IUndoRedoCommand.Execute();
            _undoStack.Push(IUndoRedoCommand);
            _redoStack.Clear();
        }

        public void Undo()
        {
            if (_undoStack.Count > 0)
            {
                IUndoRedoCommand IUndoRedoCommand = _undoStack.Pop();
                IUndoRedoCommand.Undo();
                _redoStack.Push(IUndoRedoCommand);
            }
        }

        public void Redo()
        {
            if (_redoStack.Count > 0)
            {
                IUndoRedoCommand IUndoRedoCommand = _redoStack.Pop();
                IUndoRedoCommand.Execute();
                _undoStack.Push(IUndoRedoCommand);
            }
        }
    }
}
