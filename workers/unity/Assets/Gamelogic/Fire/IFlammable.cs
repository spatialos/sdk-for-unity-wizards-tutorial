namespace Assets.Gamelogic.Fire
{
    public interface IFlammable
    {
        void OnIgnite();
        void OnExtinguish();
    }
}