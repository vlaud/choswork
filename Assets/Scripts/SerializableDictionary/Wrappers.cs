using UnityEngine;

namespace ProjectTools
{
    /// <summary>
    /// 직렬화할 수 없는 제네릭 Unity 오브젝트를 인스펙터에 노출 가능하게 하는 클래스
    /// </summary>
    /// <typeparam name="T">제네릭 타입</typeparam>
    [System.Serializable]
    public class UnityObjectWrapper<T> where T : class
    {
        [Tooltip("Unity에서 직렬화 가능한 형태로 저장하는 변수")]
        [SerializeField] private Object value;

        /// <summary>
        /// T로 캐스팅 된 value
        /// </summary>
        public T Value => value as T;

        public UnityObjectWrapper(Object value)
        {
            this.value = value;
        }
    }
}
