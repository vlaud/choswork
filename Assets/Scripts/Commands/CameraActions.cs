using System.Collections.Generic;

namespace Commands.Camera
{
    public class CameraActions : iCameraContextProvider
    {
        private static CameraActions m;

        public static CameraActions Inst => m;

        private readonly Dictionary<(CameraMode, UIMode), iCommandType<iSpringArmFunctionality>> cameraModeActions = new();
        private readonly Dictionary<UIMode, iCommandType<iSpringArmFunctionality>> uiActions = new();

        private CameraContext ctx;
        private iSpringArmFunctionality _springArmsTarget;

        private static bool IsValid => m != null && m._springArmsTarget != null;

        public CameraContext Context => m.ctx;

        public static void SetKeys(iSpringArmFunctionality iSpringArm)
        {
            if (m == null) m = new CameraActions();

            m._springArmsTarget = iSpringArm;
            m.ctx = new CameraContext();

            // FPS/TPS 명령 매핑
            m.cameraModeActions[(CameraMode.FPS, UIMode.NotUI)] = new OnGameFPS();
            m.cameraModeActions[(CameraMode.FPS, UIMode.UI)] = new OnMenuFPS();
            m.cameraModeActions[(CameraMode.TPS, UIMode.NotUI)] = new OnGameTPS();
            m.cameraModeActions[(CameraMode.TPS, UIMode.UI)] = new OnMenuTPS();

            // UI 상태 매핑
            m.uiActions[UIMode.UI] = new UI();
            m.uiActions[UIMode.NotUI] = new NotUI();

            SetMode(CameraMode.FPS);
            SetUI(UIMode.NotUI);
        }

        public static void SetMode(CameraMode mode) => m.ctx.SetMode(mode);
        public static void SetUI(UIMode ui) => m.ctx.SetUI(ui);

        /// <summary>
        /// FPS ↔ TPS 전환
        /// </summary>
        public static void ToggleCameraMode()
        {
            if (!IsValid) return;
            // ctx의 카메라 모드를 전환한다. FPS면 TPS로, 아니면 FPS로
            var newMode = m.ctx.Mode == CameraMode.FPS ? CameraMode.TPS : CameraMode.FPS;
            SetMode(newMode);
            ExecuteCameraCommand();
        }

        /// <summary>
        /// 실제로 카메라 전환을 담당하는 함수. UI 활성화 여부에 따라 커맨드가 달라짐
        /// </summary>
        public static void ExecuteCameraCommand()
        {
            // 키: 현재 ctx의 카메라 모드와, UI 활성화 여부
            var key = (m.ctx.Mode, m.ctx.UI);

            // cameraModeActions에서 키를 검색하여 커맨드 탐색
            if (m.cameraModeActions.TryGetValue(key, out var cmd))
            {
                //Debug.Log($"{m._springArmsTarget}'s command : {m.cameraModeActions[key]}");
                CommandManager.ExecuteCommand(m._springArmsTarget, cmd);
            }
        }

        /// <summary>
        /// 게임 ↔ UI 전환 
        /// </summary>
        public static void ToggleUIMode()
        {
            if (!IsValid) return;
            // ui 모드 전호나. NotUI면 UI, 아니면 NotUI
            var ui = m.ctx.UI == UIMode.NotUI ? UIMode.UI : UIMode.NotUI;
            SetUI(ui);
            ExecuteUICommand();
        }

        /// <summary>
        /// 실제로 UI 전환을 담당하는 함수
        /// </summary>
        public static void ExecuteUICommand()
        {
            if (m.uiActions.TryGetValue(m.ctx.UI, out var cmd))
            {
                //Debug.Log($"{m._springArmsTarget}'s command : {m.uiActions[m.ctx.UI]}");
                CommandManager.ExecuteCommand(m._springArmsTarget, cmd);
            }
        }
    }
}
