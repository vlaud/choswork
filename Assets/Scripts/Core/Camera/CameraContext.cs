public enum CameraMode { FPS, TPS }
public enum UIMode { UI, NotUI }

/// <summary>
/// 카메라 상태 컨텍스트 클래스
/// </summary>
public class CameraContext
{
    /// <summary>
    /// 카메라의 모드 (FPS/TPS)
    /// </summary>
    public CameraMode Mode { get; private set; }
    /// <summary>
    /// UI 활성 여부
    /// </summary>
    public UIMode UI { get; private set; }

    public void SetMode(CameraMode mode) => Mode = mode;
    public void SetUI(UIMode mode) => UI = mode;
}

