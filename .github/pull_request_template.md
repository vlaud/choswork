## 📌 개요 (Summary)
> 간단히 한 줄로 이 PR의 목적을 설명합니다.

예: Player와 Camera 간의 의존성 제거 및 테스트 가능 구조로 리팩토링

---

## 🔍 주요 변경 사항 (What has changed)

- [x] `IPlayer`, `ICameraTarget` 인터페이스 정의 (Core)
- [x] `InterfaceHolder<T>` 도입으로 유니티 인스펙터 직렬화 문제 해결
- [x] `Player.cs` 및 `CameraController.cs` 인터페이스 기반으로 의존성 변경
- [x] `PlayerMovedEvent` 도입하여 이벤트 기반 통신 처리
- [x] `.asmdef` 구조 수정 및 순환 참조 제거

---

## 🧪 테스트 (Tests)

- [x] `MockPlayer`를 통한 Player 이동 이벤트 테스트 추가
- [x] `Camera`가 이벤트에 반응하는지 확인하는 단위 테스트 추가
- [ ] (선택) Play Mode 테스트 작성 여부

---

## 📁 변경된 파일 (Affected files)

| 파일 경로 | 설명 |
|-----------|------|
| `Core/IPlayer.cs` | 플레이어 인터페이스 정의 |
| `GameLogic/Player.cs` | 인터페이스 기반 의존성 주입 구조로 리팩토링 |
| `Core/PlayerEvents.cs` | ScriptableObject 이벤트 시스템 추가 |
| `Tests/MockPlayer.cs` | 테스트용 가짜 플레이어 객체 |

---

## 💬 참고 사항 (Notes)

- 기존 `Player`가 직접 참조하던 `Camera`는 이제 인터페이스를 통해 설정됩니다.
- 인스펙터에서 인터페이스를 지정하려면 `InterfaceHolder<T>` 사용법에 주의할 것

---

## 📎 관련 이슈 (Related Issues)

- #23 순환 참조 제거
- #19 테스트 환경 분리

