using UnityEngine;

// === Interfaces ===
public interface IDependencyInjector
{
    IBaseFunctionality Resolve<T>() where T : class;
}
public interface IBaseFunctionality { }
public interface IPlayerFunctionality : IBaseFunctionality
{
    Animator GetAnimator();
}
public interface IMonsterFunctionality : IBaseFunctionality { }

public interface ISpringArmFunctionality : IBaseFunctionality { }

public interface IInvetoryFunctionality : IBaseFunctionality { }

// === Keys ===
public struct PlayerKey { }
public struct MonsterKey { }
public struct InventoryKey { }
public struct CameraKey { }
public struct UIKey { }
public struct GameKey { }
public struct SceneKey { }


