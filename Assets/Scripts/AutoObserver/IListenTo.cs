public interface IListenTo<T>
{
    /// <summary>
    /// 제너릭 타입의 이벤트를 받았을 때 호출되는 함수
    /// </summary>
    /// <param name="evt"></param>
    void OnEventReceived(T evt);
}
