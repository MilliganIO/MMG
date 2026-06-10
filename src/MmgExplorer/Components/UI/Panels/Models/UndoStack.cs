namespace MmgExplorer.Components.UI.Panels.Models;

/// <summary>
/// Lightweight undo stack for tracking reversible changes.
/// Push a snapshot before each change; Pop to restore the last one.
/// </summary>
public class UndoStack<T> where T : class
{
    private readonly int _maxDepth;
    private readonly Stack<T> _stack = new();

    public UndoStack(int maxDepth = 4)
    {
        _maxDepth = maxDepth;
    }

    public bool CanUndo => _stack.Count > 0;
    public int Count => _stack.Count;

    /// <summary>
    /// Push a snapshot onto the stack. Oldest entries are dropped when maxDepth is exceeded.
    /// </summary>
    public void Push(T snapshot)
    {
        if (_stack.Count >= _maxDepth)
        {
            // Trim from the bottom (oldest) to stay within limit.
            var items = _stack.ToArray();
            _stack.Clear();
            for (var i = 0; i < items.Length - 1; i++)
                _stack.Push(items[i]);
        }

        _stack.Push(snapshot);
    }

    /// <summary>
    /// Pop and return the most recent snapshot, or null if empty.
    /// </summary>
    public T? Pop() => _stack.Count > 0 ? _stack.Pop() : null;

    public void Clear() => _stack.Clear();
}
