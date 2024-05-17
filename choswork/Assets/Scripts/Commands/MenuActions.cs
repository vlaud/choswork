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

        private iCommandType<Mainmenu> EscapeKey;

        private iCommandType<Mainmenu> PauseAction;
        private iCommandType<Mainmenu> UnPauseAction;
        private iCommandType<Mainmenu> BackToMenu;
        private iCommandType<Mainmenu> BackToOptions;

        public static void SetKeys(Mainmenu mainmenu)
        {
            if (m == null) m = new MenuActions();

            m.PauseAction = new Pause(mainmenu);
            m.UnPauseAction = new UnPause(mainmenu);
            m.BackToMenu = new BackToMain(mainmenu);
            m.BackToOptions = new BackToOptions(mainmenu);

            m.EscapeKey = m.PauseAction;
        }

        public static void ChangeKey(MainMenuKeyType type)
        {
            switch (type)
            {
                case MainMenuKeyType.None:
                    m.EscapeKey = null;
                    break;
                case MainMenuKeyType.PauseAction:
                    m.EscapeKey = m.PauseAction;
                    break;
                case MainMenuKeyType.UnPauseAction:
                    m.EscapeKey = m.UnPauseAction;
                    break;
                case MainMenuKeyType.BackToMenu:
                    m.EscapeKey = m.BackToMenu;
                    break;
                case MainMenuKeyType.BackToOptions:
                    m.EscapeKey = m.BackToOptions;
                    break;
            }
        }

        public static void SetCommandKey(MainMenuKeyType type, iCommandType<Mainmenu> command)
        {
            switch (type)
            {
                case MainMenuKeyType.PauseAction:
                    m.PauseAction = command;
                    break;
                case MainMenuKeyType.UnPauseAction:
                    m.UnPauseAction = command;
                    break;
                case MainMenuKeyType.BackToMenu:
                    m.BackToMenu = command;
                    break;
                case MainMenuKeyType.BackToOptions:
                    m.BackToOptions = command;
                    break;
            }
        }

        public static void Execute()
        {
            CommandManager.ExecuteCommand(m.EscapeKey);
        }
    }
}