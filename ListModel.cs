using System;
using System.Collections.Generic;
using System.Linq;

namespace LimitedSizeStack;

public enum ActionType
{
    Add,
    Remove
}

public class ListModel<TItem>
{
    public List<TItem> Items { get; }
    public int UndoLimit { get; set; }

    public ListModel(int undoLimit) : this(new List<TItem>(), undoLimit)
    {
    }

    public ListModel(List<TItem> items, int undoLimit)
    {
        Items = items;
        UndoLimit = undoLimit;
    }

    public void AddItem(TItem item)
    {
        if (undoActions.Count < UndoLimit)
            Push(Items.Count - 1, ActionType.Add, item);
        else if (UndoLimit != 0 && undoActions.Count != 0)
        {
            undoActions.RemoveAt(0);
            Push(Items.Count - 1, ActionType.Add, item);
        }

        Items.Add(item);
    }

    public void RemoveItem(int index)
    {
        if (undoActions.Count < UndoLimit)
            Push(index, ActionType.Remove, Items.ElementAt(index));
        else if (UndoLimit != 0 && undoActions.Count != 0)
        {
            undoActions.RemoveAt(0);
            Push(index, ActionType.Remove, Items.ElementAt(index));
        }

        Items.RemoveAt(index);
    }

    public bool CanUndo()
    {
        return undoActions.Count > 0;
    }

    public void Undo()
    {
		if (undoActions.Count > 0)
		{
			if (undoActions[undoActions.Count - 1].Item2 == ActionType.Add)
				Items.RemoveAt(undoActions[undoActions.Count - 1].Item1 + 1);
			else
				Items.Insert(undoActions[undoActions.Count - 1].Item1, undoActions[undoActions.Count - 1].Item3);

			Pop();
		}
		else if (moveActions.Count > 0)
		{
			var lastMove = moveActions[moveActions.Count - 1];
			var item = Items[lastMove.Item2];
			Items.RemoveAt(lastMove.Item2);
			Items.Insert(lastMove.Item1, item);
			moveActions.RemoveAt(moveActions.Count - 1);
		}
    }

    private List<Tuple<int, ActionType, TItem>> undoActions = new List<Tuple<int, ActionType, TItem>>();
	private List<Tuple<int, int, TItem>> moveActions = new List<Tuple<int, int, TItem>>();

    private void Push(int number, ActionType action, TItem item)
    {
        undoActions.Add(Tuple.Create(number, action, item));
    }

    private void Pop()
    {
        if (undoActions.Count == 0) throw new InvalidOperationException();
        undoActions.RemoveAt(undoActions.Count - 1);
    }

	public void MoveUpItem(int index)
	{
		if (Items.Count <= 1) return;

		MoveItem(index, index - 1);
	}

	public void MoveDownItem(int index)
	{
		if (Items.Count <= 1) return;

		MoveItem(index, index + 1);
	}

	public void MoveItem(int oldIndex, int newIndex)
	{
		var item = Items[oldIndex];
		Items.RemoveAt(oldIndex);
		Items.Insert(newIndex, item);

		var action = Tuple.Create(oldIndex, newIndex, item);
		if (undoActions.Count < UndoLimit)
			moveActions.Add(action);
		else if (UndoLimit != 0 && moveActions.Count != 0)
		{
			moveActions.RemoveAt(0);
			moveActions.Add(action);
		}
	}

	public bool CanMove(int oldIndex, int newIndex)
	{
		return (oldIndex < 0 || oldIndex >= Items.Count ||
        newIndex < 0 || newIndex >= Items.Count);

	}
}
