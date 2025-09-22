using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 자주 사용하는 yield instruction들을 캐싱하고 제공하는 static 클래스
/// <para> WaitForEndOfFrame 사용법: <br/>
/// yield return WaitFor.EndOfFrame;</para>
/// <para> Seconds 사용법: <br/>
/// yield return WaitFor.Seconds(1f);</para>
/// </summary>
public static class WaitFor
{
    // WaitForEndOfFrame은 항상 동일하므로 하나만 캐싱
    public static readonly WaitForEndOfFrame EndOfFrame = new WaitForEndOfFrame();

    // WaitForSeconds는 초 단위로 여러개가 필요하니 Dictionary로 관리
    private static readonly Dictionary<float, WaitForSeconds> _seconds = new Dictionary<float, WaitForSeconds>();

    /// <summary>
    /// 캐싱된 WaitForSeconds를 반환한다. 없으면 새로 만들고 캐싱한다.
    /// </summary>
    /// <param name="seconds">불러올 초</param>
    /// <returns></returns>
    public static WaitForSeconds Seconds(float seconds)
    {
        if (!_seconds.ContainsKey(seconds))
        {
            _seconds[seconds] = new WaitForSeconds(seconds);
        }
        return _seconds[seconds];
    }
}
