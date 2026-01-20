using System.Collections.Generic;
using UnityEngine;

namespace FulcrumGames.Possession
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
        private readonly List<InputProvider> _boundInputProviders = new();
        public IReadOnlyList<InputProvider> BoundInputProviders => _boundInputProviders;

        private readonly List<Possessable> _possessables = new();
        public IReadOnlyList<Possessable> Possessables => _possessables;

        private Possessable _perspectivePossessable;
        public Possessable PerspectivePossessable => _perspectivePossessable;

        public void Possess(Possessable possessable)
        {
            if (_possessables.Contains(possessable))
            {
                Debug.LogWarning($"{name} was told to possess {possessable.name}, but it believes " +
                    $"that it was already possessing it!", this);
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

            _possessables.RemoveAll(item => item == null);
        }

        public void Unpossess(Possessable possessable)
        {
            if (!_possessables.Contains(possessable))
            {
                Debug.LogWarning($"{name} was told to unpossess {possessable.name}, but it believes " +
                    $"that it was already NOT possessing it!", this);
                return;
            }

            _possessables.Remove(possessable);
            possessable.OnUnpossessedBy(this);

            if (!_perspectivePossessable)
            {
                transform.parent = null;
            }

            _possessables.RemoveAll(item => item == null);
        }

        public void OnBoundToPlayer(InputProvider inputProvider)
        {
            if (_boundInputProviders.Contains(inputProvider))
            {
                Debug.LogWarning($"{name} was told to bind to {inputProvider.Name}, but it " +
                    $"believes that they are already bound!", this);
                return;
            }

            _boundInputProviders.Add(inputProvider);
            _boundInputProviders.RemoveAll(item => item == null);
        }

        public void OnUnboundFromPlayer(InputProvider inputProvider)
        {
            if (!_boundInputProviders.Contains(inputProvider))
            {
                Debug.LogWarning($"{name} was told to unbind from {inputProvider.Name}, but it " +
                    $"believes that they are NOT already bound!", this);
                return;
            }

            _boundInputProviders.Remove(inputProvider);
            _boundInputProviders.RemoveAll(item => item == null);
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
