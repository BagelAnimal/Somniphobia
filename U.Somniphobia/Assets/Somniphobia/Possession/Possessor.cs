using System.Collections.Generic;
using System.Linq;
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
        private readonly HashSet<InputProvider> _boundInputProviders = new();
        private readonly HashSet<Possessable> _possessables = new();

        private Possessable _perspectivePossessable;
        public Possessable PerspectivePossessable => _perspectivePossessable;

        public void Possess(Possessable toPossess)
        {
            _possessables.Remove(null);
            if (!toPossess)
            {
                Debug.LogWarning($"{name} was given a null possessable to possess!", this);
                return;
            }

            if (!_possessables.Add(toPossess))
                return;

            toPossess.OnPossessedBy(this);

            if (!_perspectivePossessable)
            {
                SetPerspective(toPossess);
            }
        }

        public void Unpossess(Possessable toUnpossess)
        {
            _possessables.Remove(null);
            if (!toUnpossess)
            {
                Debug.LogWarning($"{name} was given a null possessable to unpossess!", this);
                return;
            }

            if (!_possessables.Remove(toUnpossess))
                return;

            toUnpossess.OnUnpossessedBy(this);

            if (_perspectivePossessable == toUnpossess)
            {
                _perspectivePossessable = null;
                transform.parent = null;

                if (_possessables.Count > 0)
                {
                    var newPerspective = _possessables.First();
                    SetPerspective(newPerspective);
                }
            }
        }

        public void OnBoundToInputProvider(InputProvider inputProvider)
        {
            _boundInputProviders.Remove(null);
            if (inputProvider == null)
            {
                Debug.LogWarning($"{name} was given a null input provider to bind to!", this);
                return;
            }

            _boundInputProviders.Add(inputProvider);
        }

        public void OnUnboundFromInputProvider(InputProvider inputProvider)
        {
            _boundInputProviders.Remove(null);
            if (inputProvider == null)
            {
                Debug.LogWarning($"{name} was given a null input provider to unbind from!", this);
                return;
            }

            _boundInputProviders.Remove(inputProvider);
        }

        public void OnJumpPressed()
        {
            _possessables.Remove(null);
            foreach (var possessable in _possessables)
            {
                possessable.OnJumpPressed();
            }
        }

        private void SetPerspective(Possessable newPespective)
        {
            _perspectivePossessable = newPespective;
            transform.parent = _perspectivePossessable.transform;
            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}
