using System.Collections.Generic;
using UnityEngine;

namespace Assets.Somniphobia
{
    /// <summary>
    ///     A possessor is responsible for delegating inputs to some number of
    ///     possessed <see cref="Possessable"/>s. Possessors make their gameobject
    ///     a child of the oldest possessable that they have possessed. Note that
    ///     either a possessor or a possessable can initiate the process of
    ///     pairing or unpairing a possessor/possessable pair, though the codepath
    ///     will always route through the possessor's Possess() and Unpossess methods.
    ///     
    ///     These serve as a common interface for both players and CPUs to interpret
    ///     inputs into game actions.
    /// </summary>
    public class Possessor : MonoBehaviour
    {
        private readonly List<Possessable> _possessables = new();
        public IReadOnlyList<Possessable> Possessables => _possessables;

        // TECH DEBT: CPUs and players should share an interface for owning possessors.
        private readonly List<Player> _boundPlayers = new();
        public IReadOnlyList<Player> BoundPlayers => _boundPlayers;

        private Possessable _perspectivePossessable;
        public Possessable PerspectivePossessable => _perspectivePossessable;

        private void OnDestroy()
        {
            foreach (var player in _boundPlayers)
            {
                if (player == null)
                    continue;

                player.UnbindPossessor(this);
            }

            foreach (var possessable in _possessables)
            {
                if (!possessable)
                    continue;

                Unpossess(possessable);
            }
        }

        public void Possess(Possessable possessable)
        {
            if (_possessables.Contains(possessable))
            {
                Debug.LogWarning($"{name} was told to possess {possessable.name}, but it believes " +
                    $"that it was already possessing it!");
                return;
            }

            _possessables.Add(possessable);
            possessable.OnPossessedBy(this);

            var potentialPerspective = _possessables[0];
            if (potentialPerspective)
            {
                _perspectivePossessable = potentialPerspective;
                transform.parent = potentialPerspective.transform;
                transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            }
        }

        public void Unpossess(Possessable possessable)
        {
            if (!_possessables.Contains(possessable))
            {
                Debug.LogWarning($"{name} was told to unpossess {possessable.name}, but it believes " +
                    $"that it was already NOT possessing it!");
                return;
            }

            _possessables.Remove(possessable);
            possessable.OnUnpossessedBy(this);

            if (!_perspectivePossessable)
            {
                transform.parent = null;
            }
        }

        public void OnBoundToPlayer(Player player)
        {
            if (_boundPlayers.Contains(player))
            {
                Debug.LogWarning($"{name} was told to bind to {player.Name}, but it " +
                    $"believes that they are already bound!");
                return;
            }

            _boundPlayers.Add(player);
        }

        public void OnUnboundFromPlayer(Player player)
        {
            if (!_boundPlayers.Contains(player))
            {
                Debug.LogWarning($"{name} was told to unbind from {player.Name}, but it " +
                    $"believes that they are NOT already bound!");
                return;
            }

            _boundPlayers.Remove(player);
        }

        public void OnJumpPressed()
        {
            foreach (var possessable in _possessables)
            {
                if (!possessable)
                    continue;

                possessable.OnJumpPressed();
            }
        }
    }
}
