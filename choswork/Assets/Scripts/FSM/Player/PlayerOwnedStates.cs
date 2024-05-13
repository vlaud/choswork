namespace PlayerOwnedStates
{
    public class Play : State<Player>
    {
        public override void Enter(Player entity)
        {

        }

        public override void Execute(Player entity)
        {
            entity.PlayerMove();
        }

        public override void Exit(Player entity)
        {

        }

        public override bool OnMessage(Player entity, Telegram telegram)
        {
            return false;
        }
    }

    public class Pause : State<Player>
    {
        public override void Enter(Player entity)
        {

        }

        public override void Execute(Player entity)
        {

        }

        public override void Exit(Player entity)
        {

        }

        public override bool OnMessage(Player entity, Telegram telegram)
        {
            return false;
        }
    }

    public class Death : State<Player>
    {
        public override void Enter(Player entity)
        {
            entity.DeadAction();
        }

        public override void Execute(Player entity)
        {

        }

        public override void Exit(Player entity)
        {

        }

        public override bool OnMessage(Player entity, Telegram telegram)
        {
            return false;
        }
    }
}