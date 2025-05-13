using UnityEngine;

namespace Commands.Camera
{
    public enum CameraKeyType
    {
        None,
        FPS,
        TPS,
        UI,
        NotUI,
    }

    public class CameraActions
    {
        private static CameraActions m;

        public static CameraActions Inst => m;

        private iCommandType<SpringArms> VKey;
        private iCommandType<SpringArms> IKey;

        private iCommandType<SpringArms> FPSAction;
        private iCommandType<SpringArms> GameFPSAction;
        private iCommandType<SpringArms> MenuFPSAction;

        private iCommandType<SpringArms> TPSAction;
        private iCommandType<SpringArms> GameTPSAction;
        private iCommandType<SpringArms> MenuTPSAction;

        private iCommandType<SpringArms> UIAction;
        private iCommandType<SpringArms> NotUIAction;

        private CameraKeyType camState = CameraKeyType.None;
        private CameraKeyType prevState = CameraKeyType.None;

        public static void SetKeys()
        {
            if (m == null) m = new CameraActions();

            m.GameFPSAction = new OnGameFPS();
            m.MenuFPSAction = new OnMenuFPS();

            m.GameTPSAction = new OnGameTPS();
            m.MenuTPSAction = new OnMenuTPS();

            m.UIAction = new UI();
            m.NotUIAction = new NotUI();

            ChangeToMenuMode(false);
            ChangeState(CameraKeyType.FPS);
            m.prevState = CameraKeyType.TPS;
        }

        public static CameraKeyType GetState()
        {
            return m.camState;
        }

        public static void ChangeState(CameraKeyType type)
        {
            if (m.camState == type) return;

            m.prevState = m.camState;
            m.camState = type;

            switch (m.camState)
            {
                case CameraKeyType.FPS:
                    ChangeVKey(CameraKeyType.TPS);
                    break;
                case CameraKeyType.TPS:
                    ChangeVKey(CameraKeyType.FPS);
                    break;
            }
        }

        public static void ChangeToMenuMode(bool v)
        {
            if (v)
            {
                m.FPSAction = m.MenuFPSAction;
                m.TPSAction = m.MenuTPSAction;
                ChangeIKey(CameraKeyType.NotUI);
            }
            else
            {
                m.FPSAction = m.GameFPSAction;
                m.TPSAction = m.GameTPSAction;
                ChangeIKey(CameraKeyType.UI);
            }

            ChangeVKey(m.prevState);
        }

        public static void ChangeVKey(CameraKeyType type)
        {
            switch (type)
            {
                case CameraKeyType.FPS:
                    m.VKey = m.FPSAction;
                    break;
                case CameraKeyType.TPS:
                    m.VKey = m.TPSAction;
                    break;
            }
        }

        public static void ChangeIKey(CameraKeyType type)
        {
            switch (type)
            {
                case CameraKeyType.UI:
                    m.IKey = m.UIAction;
                    break;
                case CameraKeyType.NotUI:
                    m.IKey = m.NotUIAction;
                    break;
            }

        }

        public static void ExecuteVKey()
        {
            CommandManager.ExecuteCommand(m.VKey);
        }

        public static void ExecuteIKey()
        {
            CommandManager.ExecuteCommand(m.IKey);
        }
    }
}