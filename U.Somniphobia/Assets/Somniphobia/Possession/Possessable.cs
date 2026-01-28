using System;
using System.Collections.Generic;
using UnityEngine;

namespace FulcrumGames.Possession
{
    /// <summary>
    ///     Possessables are targeted by some number of <see cref="Possessor"/>s,
    ///     and then parse input from those possessors into actions that can be
    ///     delegated to a series of components, i.e., a jump input into force.
    ///     
    ///     <see cref="Possessor"/>s are expected to be controlled by <see cref="InputProvider"/>s.
    /// </summary>
    public class Possessable : MonoBehaviour
    {
        /// <summary>
        ///     Invoked when a possessor possesses this.
        /// </summary>
        public event Action<Possessor> PossessedBy;

        /// <summary>
        ///     Invoked when a possessor unpossesses this.
        /// </summary>
        public event Action<Possessor> UnpossessedBy;

        public event Action Jump;

        private readonly List<Possessor> _possessors = new();
        /// <summary>
        ///     The <see cref="Possessor"/>s currently delegating inputs to this possessable.
        ///     Cannot contain duplicates.
        /// </summary>
        public IReadOnlyList<Possessor> Possessors => _possessors;

        // Used to avoid reacting to unbinding when we're the one invoking it in loop.
        private bool _isBeingDestroyed = false;

        private void OnDestroy()
        {
            _isBeingDestroyed = true;
            _possessors.Remove(null);
            foreach (var possessor in _possessors)
            {
                possessor.Unpossess(this);
            }
        }

        /// <summary>
        ///     Call when adding a <see cref="Possessor"/> to a given possessable.
        ///     Lets the possessable track it, thus giving it the opportunity to let
        ///     the <see cref="Possessor"/> know that it is being destroyed.
        /// </summary>
        public void OnPossessedBy(Possessor possessor)
        {
            _possessors.Remove(null);
            if (!possessor)
            {
                Debug.LogWarning($"{name} was given a null possessor to be possessed by!",
                    this);
                return;
            }

            if (_possessors.Contains(possessor))
            {
                Debug.LogWarning($"{name} is already bound to {possessor.name}!", this);
                return;
            }

            _possessors.Add(possessor);
            PossessedBy?.Invoke(possessor);
            foreach (var inputProvider in possessor.BoundInputProviders)
            {
                inputProvider.Jump += Jump;
            }
            possessor.BoundBy += OnBoundBy;
            possessor.UnboundBy += OnUnboundBy;
        }

        /// <summary>
        ///     Call when removing a <see cref="Possessor"/> from a given possessable.
        ///     Lets the possessable track it, thus helping it avoid a case where it
        ///     tries to let an uncaring or null <see cref="Possessor"/> know that
        ///     it is being destroyed.
        /// </summary>
        public void OnUnpossessedBy(Possessor possessor)
        {
            if (!possessor)
            {
                Debug.LogWarning($"{name} was given a null possessor to be unpossessed from!",
                    this);
                return;
            }

            if (!_possessors.Contains(possessor))
            {
                Debug.LogWarning($"{name} is already NOT bound to {possessor.name}!", this);
                return;
            }

            if (!_isBeingDestroyed)
            {
                _possessors.Remove(null);
                _possessors.Remove(possessor);
            }

            UnpossessedBy?.Invoke(possessor);
            foreach (var inputProvider in possessor.BoundInputProviders)
            {
                inputProvider.Jump -= Jump;
            }
            possessor.BoundBy -= OnBoundBy;
            possessor.UnboundBy -= OnUnboundBy;
        }

        /// <summary>
        ///     Returns the sum of all look input provided by possessors.
        /// </summary>
        public Vector3 GetLookInput()
        {
            Vector3 lookInput = default;
            foreach (var possessor in Possessors)
            {
                foreach (var inputProvider in possessor.BoundInputProviders)
                {
                    lookInput += inputProvider.GetLookInput();
                }
            }

            return lookInput;
        }

        private void OnBoundBy(InputProvider inputProvider)
        {
            inputProvider.Jump += Jump;
        }

        private void OnUnboundBy(InputProvider inputProvider)
        {
            inputProvider.Jump -= Jump;
        }
    }
}
