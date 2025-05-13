namespace Commands.Menu
{
    public class Pause : iCommandType<Mainmenu>
    {
        private Mainmenu _mainMenu;

        public Pause(Mainmenu mainMenu)
        {
            _mainMenu = mainMenu;
        }

        public void Execute()
        {
            GameStatesEvent.Trigger(GameEventType.Pause);
            MenuActions.ChangeKey(MainMenuKeyType.UnPauseAction);
            _mainMenu?.ShowMenuAnim(true);
        }

        public void Undo()
        {
            GameStatesEvent.Trigger(GameEventType.UnPause);
            MenuActions.ChangeKey(MainMenuKeyType.PauseAction);

            _mainMenu?.ShowMenuAnim(false);
            _mainMenu?.DisableUI();
        }
    }

    public class UnPause : iCommandType<Mainmenu>
    {
        private Mainmenu _mainMenu;

        public UnPause(Mainmenu mainMenu)
        {
            _mainMenu = mainMenu;
        }

        public void Execute()
        {
            GameStatesEvent.Trigger(GameEventType.UnPause);
            MenuActions.ChangeKey(MainMenuKeyType.PauseAction);

            _mainMenu?.ShowMenuAnim(false);
            _mainMenu?.DisableUI();
        }

        public void Undo()
        {
            GameStatesEvent.Trigger(GameEventType.Pause);
            MenuActions.ChangeKey(MainMenuKeyType.UnPauseAction);
            _mainMenu?.ShowMenuAnim(true);
        }
    }

    public class BackToMain : iCommandType<Mainmenu>
    {
        private Mainmenu _mainMenu;

        public BackToMain(Mainmenu mainMenu)
        {
            _mainMenu = mainMenu;
        }

        public void Execute()
        {
            _mainMenu?.back_options();
        }

        public void Undo()
        {

        }
    }

    public class BackToOptions : iCommandType<Mainmenu>
    {
        private Mainmenu _mainMenu;

        public BackToOptions(Mainmenu mainMenu)
        {
            _mainMenu = mainMenu;
        }

        public void Execute()
        {
            _mainMenu?.back_options_panels();
        }

        public void Undo()
        {

        }
    }
}