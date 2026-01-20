using System.Collections.Generic;

namespace FulcrumGames.Possession
{
    public abstract class InputProvider
    {
        private string _name = "CPU";
        public string Name => _name;

        private readonly List<Possessor> _possessors = new();
        public IReadOnlyList<Possessor> Possessors => _possessors;

        public void SetName(string name)
        {
            _name = name;
        }

        public void BindToPossessor(Possessor possessor)
        {
            if (_possessors.Contains(possessor))
            {
                UnityEngine.Debug.LogWarning($"{_name} was told to add {possessor.name} to " +
                    $"its possessor list, but it thinks that it's already possessing it!");
                return;
            }

            _possessors.Add(possessor);

            possessor.OnBoundToPlayer(this);
        }

        public void UnbindFromPossessor(Possessor possessor)
        {
            if (!_possessors.Contains(possessor))
            {
                UnityEngine.Debug.LogWarning($"{_name} was told to remove {possessor.name} " +
                    $"from its possessor list, but it thinks that it's already NOT possessing it!");
                return;
            }

            _possessors.Remove(possessor);

            possessor.OnUnboundFromPlayer(this);
        }

        internal void JumpPressed()
        {
            foreach (var possessor in _possessors)
            {
                if (!possessor)
                    continue;

                possessor.OnJumpPressed();
            }
        }
    }
}
