namespace Commands.Camera
{
    public class OnGameFPS : iCommandType<SpringArms>
    {
        public void Execute()
        {
            CameraStatesEvent.Trigger(CameraEventType.FPS);
            CameraActions.ChangeState(CameraKeyType.FPS);
        }

        public void Undo()
        {

        }
    }

    public class OnMenuFPS : iCommandType<SpringArms>
    {
        public void Execute()
        {
            CameraActions.ChangeState(CameraKeyType.FPS);
        }

        public void Undo()
        {

        }
    }

    public class OnGameTPS : iCommandType<SpringArms>
    {
        public void Execute()
        {
            CameraStatesEvent.Trigger(CameraEventType.TPS);
            CameraActions.ChangeState(CameraKeyType.TPS);
        }

        public void Undo()
        {

        }
    }

    public class OnMenuTPS : iCommandType<SpringArms>
    {
        public void Execute()
        {
            CameraActions.ChangeState(CameraKeyType.TPS);
        }

        public void Undo()
        {

        }
    }

    public class UI : iCommandType<SpringArms>
    {
        public void Execute()
        {
            CameraStatesEvent.Trigger(CameraEventType.UI);
            CameraActions.ChangeToMenuMode(true);
        }

        public void Undo()
        {

        }
    }

    public class NotUI : iCommandType<SpringArms>
    {
        public void Execute()
        {
            CameraStatesEvent.Trigger(CameraEventType.NotUI);
            CameraActions.ChangeToMenuMode(false);
        }

        public void Undo()
        {

        }
    }
}