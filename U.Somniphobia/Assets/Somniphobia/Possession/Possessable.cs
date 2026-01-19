using System.Collections.Generic;
using UnityEngine;

namespace FulcrumGames.Possession
{
    /// <summary>
    ///     Possessables are targeted by some number of <see cref="Possessor"/>s,
    ///     and then parse input from those possessors into actions that can be
    ///     delegated to a series of components, i.e., a jump input into force.
    ///     
    ///     <see cref="Possessor"/>s are expected to be controlled by <see cref="Player"/>s
    ///     or CPUs.
    /// </summary>
    public class Possessable : MonoBehaviour
    {
        private readonly List<Possessor> _possessors = new();
        public IReadOnlyList<Possessor> Possessors => _possessors;

        private void OnDestroy()
        {
            foreach (var possessor in _possessors)
            {
                if (!possessor)
                    continue;

                possessor.Unpossess(this);
            }
        }

        public void OnPossessedBy(Possessor possessor)
        {
            if (_possessors.Contains(possessor))
            {
                Debug.LogWarning($"{nameof(Possessor)} {name} was notifieid that " +
                    $"it was possessed by {possessor.name}, but it believes that it " +
                    $"was already being possessed by it.", this);
                return;
            }

            _possessors.Add(possessor);
        }

        public void OnUnpossessedBy(Possessor possessor)
        {
            if (!_possessors.Contains(possessor))
            {
                Debug.LogWarning($"{nameof(Possessor)} {name} was notifieid that " +
                    $"it was unpossessed by {possessor.name}, but it believes that it " +
                    $"was already NOT being possessed by it.", this);
                return;
            }

            _possessors.Remove(possessor);
        }

        public void OnJumpPressed()
        {
            Debug.Log("Jumped!");
        }
    }
}
