using UnityEngine;

// === Interfaces ===
public interface IDependencyInjector
{
    iBaseFunctionality Resolve<T>() where T : class;
}
public interface iBaseFunctionality { }
public interface iPlayerFunctionality : iBaseFunctionality
{
    Animator GetAnimator();
}
public interface iMonsterFunctionality : iBaseFunctionality { }

public interface iSpringArmFunctionality : iBaseFunctionality { }

public interface iInvetoryFunctionality : iBaseFunctionality { }
public interface iMainmenuFunctionality : iBaseFunctionality
{
    void ShowMenuAnim(bool v);
    void DisableUI();
    void back_options();
    void back_options_panels();
}

// === Keys ===
public struct PlayerKey { }
public struct MonsterKey { }
public struct InventoryKey { }
public struct CameraKey { }
public struct UIKey { }
public struct GameKey { }
public struct SceneKey { }


