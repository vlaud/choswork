using System;
using UnityEngine;

public class CutScenePlayer : MonoBehaviour, iUpdateActionFunctionality
{
    public Animator anim;
    public Animator CarAnim;
    public Animator CarDoorAnim;

    /// <summary>
    /// 컷신 업데이트 액션 등록
    /// </summary>
    private Action UpdateAction;

    void Update()
    {
        UpdateAction?.Invoke();
    }

    /// <summary>
    /// 차에서 나오면 플레이어 트랜스폼의 부모를 null로 설정 <br/>
    /// (플레이어가 차에서 나옴)
    /// </summary>
    public void CarOut()
    {
        transform.SetParent(null);
    }

    /// <summary>
    /// 다음 신으로 넘어감
    /// </summary>
    public void FadeToLevel()
    {
        GameManagement.Inst.IsGameClear = true;
        GameManagement.Inst.GameClear();
    }

    /// <summary>
    /// 컷신 업데이트 액션 등록
    /// </summary>
    /// <param name="action">등록할 액션</param>
    public void SetUpdateAction(Action action)
    {
        UpdateAction -= action;
        UpdateAction += action;
    }
    
    /// <summary>
    /// 컷신 업데이트 액션 해제
    /// </summary>
    /// <param name="action">해제할 액션</param>
    public void ReleaseUpdateAction(Action action)
    {
        UpdateAction -= action;
    }
}
