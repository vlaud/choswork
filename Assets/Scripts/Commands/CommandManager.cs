using System;
using System.Collections.Generic;
using UnityEngine;

public interface iCommandBase { }

public interface iCommandType<T> : iCommandBase
{
    void Execute(T target);
    void Undo(T target);
}

/// <summary>
/// 커맨드들을 관리하는 매니저
/// </summary>
public class CommandManager
{
    // 실행한 명령을 취소할 명령들을 저장하는 스택
    private static readonly Dictionary<Type, object> _undoList = new();

    // second stack of redoable commands
    // 취소한 명령들을 다시 되돌릴 수 있는 명령들을 저장하는 스택
    private static readonly Dictionary<Type, object> _redoList = new();

    /// <summary>
    /// undoList와 redoList를 초기화합니다.
    /// </summary>
    public static void ClearCommands()
    {
        _undoList.Clear();
        _redoList.Clear();
    }

    /// <summary>
    /// 명령을 실행하면서 undoList에 추가합니다.
    /// </summary>
    /// <typeparam name="T">제너릭 타입</typeparam>
    /// <param name="target">실행할 개체</param>
    /// <param name="command">명령</param>
    public static void ExecuteCommand<T>(T target, iCommandType<T> command) where T : iBaseFunctionality
    {
        // 커맨드가 null이면 아무것도 하지 않음
        if (command == null) return;

        // 개체가 명령을 실행함
        command.Execute(target);

        // 타입을 가져옴
        Type type = typeof(T);

        // undoList에 타입이 없으면 새로운 스택을 생성
        if (!_undoList.TryGetValue(type, out var obj))
        {
            obj = new Stack<iCommandType<T>>();
            _undoList[type] = obj;
        }

        var _undo = obj as Stack<iCommandType<T>>;

        // 명령을 undoList에 추가
        _undo.Push(command);

        // redoList에 타입이 없으면 아무것도 하지 않음
        if (!_redoList.ContainsKey(type)) return;

        // redoList에 타입이 있으면 스택을 비움
        var _redo = _redoList[type] as Stack<iCommandType<T>>;
        _redo.Clear();
    }

    public static void UndoCommand<T>(T target) where T : iBaseFunctionality
    {
        Type type = typeof(T);

        if (!_undoList.ContainsKey(type)) return;
        var _undo = _undoList[type] as Stack<iCommandType<T>>;

        if (_undo.Count > 0)
        {
            iCommandType<T> activeCommand = _undo.Pop();

            if (!_redoList.TryGetValue(type, out var obj))
            {
                obj = new Stack<iCommandType<T>>();
                _redoList[type] = obj;
            }
            var stack = obj as Stack<iCommandType<T>>;
            stack.Push(activeCommand);

            activeCommand.Undo(target);
        }
    }

    public static void RedoCommand<T>(T target) where T : iBaseFunctionality
    {
        Type type = typeof(T);

        if (!_redoList.ContainsKey(type)) return;

        var _redo = _redoList[type] as Stack<iCommandType<T>>;
        if (_redo.Count > 0)
        {
            iCommandType<T> activeCommand = _redo.Pop();

            var _undo = _undoList[type] as Stack<iCommandType<T>>;
            _undo.Push(activeCommand);
            activeCommand.Execute(target);
        }
    }
}
