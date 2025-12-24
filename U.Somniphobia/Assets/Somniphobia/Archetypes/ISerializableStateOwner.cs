namespace FulcrumGames.Somniphobia.Archetypes
{
    public interface ISerializableStateOwner
    {
        public ISerializableState GetState();
        public void SetState(ISerializableState state);
    }
}
