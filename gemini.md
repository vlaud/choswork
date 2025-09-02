- 코루틴을 사용해야 할때는 CoroutineRunner.cs 파일의 CoroutineRunner 클래스 사용
- Debug를 사용할때는 Debug.cs 파일의 public static class Debug 사용
- 특정 인터페이스를 구현하는 오브젝트를 찾을때는 ComponentTypeFinder.cs 클래스 사용
- 이벤트 버스를 사용할 때는 GameEventManager.cs 와 <이벤트 타입>EventsType.cs 사용
예시) ObjectEventType.cs
- 인스펙터에 클래스를 직접 등록하는 대신, InterfaceHolder.cs를 이용한 간접 등록
