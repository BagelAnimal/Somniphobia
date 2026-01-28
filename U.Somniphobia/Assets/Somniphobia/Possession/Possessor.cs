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
        /// <summary>
        ///     The providers currently delegating inputs to this possessor.
        ///     Cannot contain duplicates.
        /// </summary>
        public IReadOnlyList<InputProvider> BoundInputProviders => _boundInputProviders;

        private readonly List<Possessable> _possessables = new();
        /// <summary>
        ///     The possessables currently receiving inputs delegated to this possessor.
        ///     Cannot contain duplicates.
        /// </summary>
        public IReadOnlyList<Possessable> Possessables => _possessables;

        private Possessable _perspectivePossessable;
        /// <summary>
        ///     The first <see cref="Possessable"/> used as a parent. Useful when this
        ///     possessor is an inworld object like a soul and it needs a place to live.
        ///     Will always be set as long as at least one possessable is bound.
        /// </summary>
        public Possessable PerspectivePossessable => _perspectivePossessable;

        // used to avoid modifying collections while unhooking input providers on destroy
        private bool _isBeingDestroyed = false;

        private void OnDestroy()
        {
            _isBeingDestroyed = true;

            foreach (var inputProvider in _boundInputProviders)
            {
                inputProvider.UnbindFromPossessor(this);
            }
        }

        /// <summary>
        ///     Begin routing inputs to the provided <see cref="Possessable"/>.
        /// </summary>
        public void Possess(Possessable toPossess)
        {
            PruneNulls();
            if (!toPossess)
            {
                Debug.LogWarning($"{name} was given a null possessable to possess!", this);
                return;
            }

            if (_possessables.Contains(toPossess))
            {
                Debug.LogWarning($"{name} is already possessing {toPossess.name}!", this);
                return;
            }

            _possessables.Add(toPossess);
            toPossess.OnPossessedBy(this);

            if (!_perspectivePossessable)
            {
                SetPerspective(toPossess);
            }
        }

        /// <summary>
        ///     Cease routing inputs to the provided <see cref="Possessable"/>.
        /// </summary>
        public void Unpossess(Possessable toUnpossess)
        {
            PruneNulls();
            if (!toUnpossess)
            {
                Debug.LogWarning($"{name} was given a null possessable to unpossess!", this);
                return;
            }

            if (!_possessables.Contains(toUnpossess))
            {
                Debug.LogWarning($"{name} is already NOT possessing {toUnpossess.name}!", this);
                return;
            }

            _possessables.Remove(toUnpossess);
            toUnpossess.OnUnpossessedBy(this);

            if (_perspectivePossessable == toUnpossess)
            {
                _perspectivePossessable = null;

                // Transform might be null if we are in the process of being destroyed.
                if (!_isBeingDestroyed)
                {
                    transform.parent = null;
                }

                if (_possessables.Count > 0)
                {
                    var newPerspective = _possessables[0];
                    SetPerspective(newPerspective);
                }
            }
        }

        /// <summary>
        ///     Track an input provider so this can let it know when it is being destroyed.
        /// </summary>
        public void OnBoundToInputProvider(InputProvider inputProvider)
        {
            if (_isBeingDestroyed)
                return;

            PruneNulls();
            if (inputProvider == null)
            {
                Debug.LogWarning($"{name} was given a null input provider to bind to!", this);
                return;
            }

            if (_boundInputProviders.Contains(inputProvider))
            {
                Debug.LogWarning($"{name} is already bound to {inputProvider.Name}!", this);
                return;
            }

            _boundInputProviders.Add(inputProvider);
        }

        /// <summary>
        ///     Track when an input provider stops caring about this so we don't tell them
        ///     when we are being destroyed.
        /// </summary>
        public void OnUnboundFromInputProvider(InputProvider inputProvider)
        {
            if (_isBeingDestroyed)
                return;

            PruneNulls();
            if (inputProvider == null)
            {
                Debug.LogWarning($"{name} was given a null input provider to unbind from!", this);
                return;
            }

            if (!_boundInputProviders.Contains(inputProvider))
            {
                Debug.LogWarning($"{name} is already NOT bound to {inputProvider.Name}!", this);
                return;
            }

            _boundInputProviders.Remove(inputProvider);
        }

        /// <summary>
        ///     Delegate a jump input from a provider down to all possessed
        ///     <see cref="Possessable"/>s.
        /// </summary>
        public void OnJumpPressed()
        {
            PruneNulls();
            foreach (var possessable in _possessables)
            {
                possessable.OnJumpPressed();
            }
        }

        private void PruneNulls()
        {
            _boundInputProviders.Remove(null);
            _possessables.Remove(null);
        }

        private void SetPerspective(Possessable newPerspective)
        {
            var rigidbody = GetComponent<Rigidbody>();
            var collider = GetComponent<Collider>();

            if (!newPerspective)
            {
                // unparent, then enable physics on this.
                _perspectivePossessable = null;
                transform.parent = null;
                if (rigidbody)
                {
                    rigidbody.isKinematic = false;
                }
                if (collider)
                {
                    collider.enabled = true;
                }

                return;
            }

            _perspectivePossessable = newPerspective;

            // parent, then disable physics on this.
            var anchor = _perspectivePossessable.GetComponentInChildren<PossessorAnchor>();
            var parent = anchor ? anchor.transform : _perspectivePossessable.transform;
            transform.parent = parent;

            transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            if (rigidbody)
            {
                rigidbody.isKinematic = true;
            }
            if (collider)
            {
                collider.enabled = false;
            }
        }
    }
}
