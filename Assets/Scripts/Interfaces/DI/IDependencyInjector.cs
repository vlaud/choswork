using System;
using UnityEngine;

// === Interfaces ===
public interface IDependencyInjector
{
    iBaseFunctionality Resolve<T>() where T : class;
}
public interface iBaseFunctionality { }

/// <summary>
/// Update 함수 내에서 돌아갈 액션들을 관리
/// </summary>
public interface iUpdateActionFunctionality : iBaseFunctionality
{
    /// <summary>
    /// 업데이트 액션 등록
    /// </summary>
    /// <param name="action">등록할 액션</param>
    void SetUpdateAction(Action action);

    /// <summary>
    /// 업데이트 액션 해제
    /// </summary>
    /// <param name="action">해제할 액션</param>
    void ReleaseUpdateAction(Action action);
}

public interface iPlayerFunctionality : iBaseFunctionality
{
    Animator GetAnimator();
}
public interface iMonsterFunctionality : iBaseFunctionality { }

public interface iSpringArmFunctionality : iBaseFunctionality { }

public interface iInventoryFunctionality : iBaseFunctionality 
{
    bool IsItemExist(Item item);
    void DestroyItem(Item item);
}
public interface iMainmenuFunctionality : iBaseFunctionality
{
    void ShowMenuAnim(bool v);
    void DisableUI();
    void back_options();
    void back_options_panels();
}

public interface iTimeFunctionality : iBaseFunctionality
{
    void Pause();
    void UnPause();
    float GameTimeScale { get; }
    bool IsSlowing { get; }
}

// === Keys ===
public struct PlayerKey { }
public struct MonsterKey { }
public struct InventoryKey { }
public struct CameraKey { }
public struct UIKey { }
public struct GameKey { }
public struct SceneKey { }


