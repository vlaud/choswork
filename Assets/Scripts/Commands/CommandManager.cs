using System;
using System.Collections.Generic;
using UnityEngine;

public interface iCommandBase
{
    void Execute();
    void Undo();
}

public interface iCommandType<T> : iCommandBase { }

public class CommandManager
{
    // stack of command objects to undo
    private static readonly Dictionary<Type, Stack<iCommandBase>> _undoList
        = new Dictionary<Type, Stack<iCommandBase>>();

    // second stack of redoable commands
    private static readonly Dictionary<Type, Stack<iCommandBase>> _redoList
        = new Dictionary<Type, Stack<iCommandBase>>();

    public static void ClearCommands()
    {
        _undoList.Clear();
        _redoList.Clear();
    }

    // execute a command object directly and save to the undo stack
    public static void ExecuteCommand<T>(iCommandType<T> command) where T : MonoBehaviour
    {
        if (command == null) return;

        command.Execute();

        Type type = typeof(T);

        if (!_undoList.ContainsKey(type))
        {
            _undoList[type] = new Stack<iCommandBase>();
        }

        _undoList[type].Push(command);

        // clear out the redo stack if we make a new move
        if (!_redoList.ContainsKey(type)) return;
        _redoList[type].Clear();
    }

    public static void UndoCommand<T>() where T : MonoBehaviour
    {
        Type type = typeof(T);

        if (!_undoList.ContainsKey(type)) return;

        if (_undoList[type].Count > 0)
        {
            iCommandBase activeCommand = _undoList[type].Pop();

            if (!_redoList.ContainsKey(type))
            {
                _redoList[type] = new Stack<iCommandBase>();
            }
            _redoList[type].Push(activeCommand);
            activeCommand.Undo();
        }
    }

    public static void RedoCommand<T>() where T : MonoBehaviour
    {
        Type type = typeof(T);

        if (!_redoList.ContainsKey(type)) return;

        if (_redoList[type].Count > 0)
        {
            iCommandBase activeCommand = _redoList[type].Pop();
            _undoList[type].Push(activeCommand);
            activeCommand.Execute();
        }
    }
}