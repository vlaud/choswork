namespace Commands.Camera
{
    /// <summary>
    /// 게임 상태 시 FPS로 전환
    /// </summary>
    public class OnGameFPS : iCommandType<iSpringArmFunctionality>
    {
        public void Execute(iSpringArmFunctionality target)
        {
            CameraStatesEvent.Trigger(CameraEventType.FPS);
        }

        public void Undo(iSpringArmFunctionality target)
        {
            
        }
    }

    /// <summary>
    /// UI 활성화시 FPS로 미리 전환
    /// </summary>
    public class OnMenuFPS : iCommandType<iSpringArmFunctionality>
    {
        public void Execute(iSpringArmFunctionality target)
        {

        }

        public void Undo(iSpringArmFunctionality target)
        {

        }
    }

    /// <summary>
    /// 게임 상태 시 TPS로 전환
    /// </summary>
    public class OnGameTPS : iCommandType<iSpringArmFunctionality>
    {
        public void Execute(iSpringArmFunctionality target)
        {
            CameraStatesEvent.Trigger(CameraEventType.TPS);
        }

        public void Undo(iSpringArmFunctionality target)
        {

        }
    }

    /// <summary>
    /// UI 활성화시 TPS로 미리 전환
    /// </summary>
    public class OnMenuTPS : iCommandType<iSpringArmFunctionality>
    {
        public void Execute(iSpringArmFunctionality target)
        {

        }

        public void Undo(iSpringArmFunctionality target)
        {

        }
    }

    /// <summary>
    /// UI 활성화
    /// </summary>
    public class UI : iCommandType<iSpringArmFunctionality>
    {
        public void Execute(iSpringArmFunctionality target)
        {
            CameraStatesEvent.Trigger(CameraEventType.UI);
        }


        public void Undo(iSpringArmFunctionality target)
        {

        }
    }

    /// <summary>
    /// UI 비활성화
    /// </summary>
    public class NotUI : iCommandType<iSpringArmFunctionality>
    {
        public void Execute(iSpringArmFunctionality target)
        {
            CameraStatesEvent.Trigger(CameraEventType.NotUI);
        }

        public void Undo(iSpringArmFunctionality target)
        {

        }
    }
}
