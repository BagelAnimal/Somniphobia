using System;

namespace FulcrumGames.Somniphobia.Archetypes
{
    public interface ISerializableState
    {
        public Type GetOwnerType();
    }
}
