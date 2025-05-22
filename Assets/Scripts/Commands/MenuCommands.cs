namespace Commands.Menu
{
    public class Pause : iCommandType<iMainmenuFunctionality>
    {
        public void Execute(iMainmenuFunctionality target)
        {
            GameStatesEvent.Trigger(GameEventType.Pause);
            MenuActions.ChangeKey(MainMenuKeyType.UnPauseAction);
            target?.ShowMenuAnim(true);
        }

        public void Undo(iMainmenuFunctionality target)
        {
            GameStatesEvent.Trigger(GameEventType.UnPause);
            MenuActions.ChangeKey(MainMenuKeyType.PauseAction);

            target?.ShowMenuAnim(false);
            target?.DisableUI();
        }
    }

    public class UnPause : iCommandType<iMainmenuFunctionality>
    {
        public void Execute(iMainmenuFunctionality target)
        {
            GameStatesEvent.Trigger(GameEventType.UnPause);
            MenuActions.ChangeKey(MainMenuKeyType.PauseAction);

            target?.ShowMenuAnim(false);
            target?.DisableUI();
        }


        public void Undo(iMainmenuFunctionality target)
        {
            GameStatesEvent.Trigger(GameEventType.Pause);
            MenuActions.ChangeKey(MainMenuKeyType.UnPauseAction);
            target?.ShowMenuAnim(true);
        }
    }

    public class BackToMain : iCommandType<iMainmenuFunctionality>
    {
        public void Execute(iMainmenuFunctionality target)
        {
            target?.back_options();
        }


        public void Undo(iMainmenuFunctionality target)
        {
        }
    }

    public class BackToOptions : iCommandType<iMainmenuFunctionality>
    {
        public void Execute(iMainmenuFunctionality target)
        {
            target?.back_options_panels();
        }


        public void Undo(iMainmenuFunctionality target)
        {

        }
    }
}
