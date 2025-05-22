using System.Collections.Generic;

namespace Commands.Menu
{
    public enum MainMenuKeyType
    {
        None,
        PauseAction,
        UnPauseAction,
        BackToMenu,
        BackToOptions,
    }

    public class MenuActions
    {
        private static MenuActions m;
        public static MenuActions Inst => m;

        private iMainmenuFunctionality _mainMenuTarget;

        private MainMenuKeyType currentKey = MainMenuKeyType.PauseAction;

        private readonly Dictionary<MainMenuKeyType, iCommandType<iMainmenuFunctionality>> commands = new();
        /// <summary>
        /// 사용 가능 상태 : 인스턴스가 존재하고 타겟이 null이 아닐 때
        /// </summary>
        private static bool IsValid => m != null && m._mainMenuTarget != null;

        public static void SetKeys(iMainmenuFunctionality mainmenu)
        {
            if (m == null) m = new MenuActions();

            m._mainMenuTarget = mainmenu;

            m.commands.TryAdd(MainMenuKeyType.PauseAction, new Pause());
            m.commands.TryAdd(MainMenuKeyType.UnPauseAction, new UnPause());
            m.commands.TryAdd(MainMenuKeyType.BackToMenu, new BackToMain());
            m.commands.TryAdd(MainMenuKeyType.BackToOptions, new BackToOptions());

            m.currentKey = MainMenuKeyType.PauseAction;
        }

        public static void ChangeKey(MainMenuKeyType type)
        {
            //if (!IsValid || !m.commands.ContainsKey(type)) return;
            m.currentKey = type;
        }

        /// <summary>
        /// 커맨드 키를 설정합니다.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="command"></param>
        public static void SetCommandKey(MainMenuKeyType type, iCommandType<iMainmenuFunctionality> command)
        {
            // 타깃이 없거나 인스턴스가 없으면 아무것도 하지 않음
            //if (!IsValid) return;

            // MainMenuKeyType에 따라 명령을 설정
            m.commands[type] = command;
        }

        public static void Execute()
        {
            //Debug.Log($"{m._mainMenuTarget}'s command : {m.commands[m.currentKey]}");
            //if (!IsValid) return;
            CommandManager.ExecuteCommand(m._mainMenuTarget, m.commands[m.currentKey]);
        }

        public static void Reset()
        {
            if (m == null) return;
            m.commands.Clear();
            m._mainMenuTarget = null;
        }
    }
}
