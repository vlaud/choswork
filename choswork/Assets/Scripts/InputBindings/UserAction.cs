namespace Rito.InputBindings
{
    /// <summary> 사용자 입력에 의한 행동 정의 </summary>
    public enum UserAction
    {
        // 오브젝트 액션
        Throw,

        // 플레이어 액션
        Attack,
        Run,
        Interact,
        UseItem,
        
        // UI
        Camera,
        Debug,
        UI_Inventory,
        Escape,

        // 저널
        JournalLeft,
        JournalRight,
    }
}