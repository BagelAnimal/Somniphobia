using System;
using System.Collections.Generic;
using UnityEngine;

namespace FulcrumGames.Possession
{
    /// <summary>
    ///     Objects like CPUs and players are meant to inherit from or use objects that
    ///     inherit from this, where this class's intent is to serve as a consistent
    ///     interface for owning <see cref="Possessor"/>s, and for passing inputs to
    ///     those <see cref="Possessor"/>s so that they can mutate state on their
    ///     corresponding <see cref="Possessable"/>s.
    /// </summary>
    public abstract class InputProvider
    {
        public event Action Jump;

        private string _name = "CPU";
        public string Name => _name;

        protected readonly List<Possessor> _possessors = new();
        /// <summary>
        ///     The <see cref="Possessor"/>s currently receiving inputs from this provider.
        ///     Cannot contain duplicates.
        /// </summary>
        public IReadOnlyList<Possessor> Possessors => _possessors;

        /// <summary>
        ///     Give this guy a name.
        /// </summary>
        public void SetName(string name)
        {
            _name = name;
        }

        /// <summary>
        ///     Begin routing all inputs associated with this provider to the
        ///     <see cref="Possessor"/>.
        /// </summary>
        public void BindToPossessor(Possessor possessor)
        {
            _possessors.Remove(null);
            if (!possessor)
            {
                UnityEngine.Debug.LogWarning($"{_name} was given a null possessor to bind to!");
                return;
            }

            if (_possessors.Contains(possessor))
            {
                UnityEngine.Debug.LogWarning($"{_name} is already bound to" +
                    $"{possessor.name}!", possessor);
                return;
            }

            _possessors.Add(possessor);
            possessor.OnBoundToInputProvider(this);
        }

        /// <summary>
        ///     Cease routing all inputs associated with this provider to the
        ///     <see cref="Possessor"/>.
        /// </summary>
        public void UnbindFromPossessor(Possessor possessor)
        {
            _possessors.Remove(null);
            if (!possessor)
            {
                UnityEngine.Debug.LogWarning($"{_name} was given a null possessor to unbind" +
                    $"from!");
                return;
            }

            if (!_possessors.Contains(possessor))
            {
                UnityEngine.Debug.LogWarning($"{_name} is already NOT bound to" +
                    $"{possessor.name}!", possessor);
                return;
            }

            _possessors.Remove(possessor);
            possessor.OnUnboundFromInputProvider(this);
        }

        /// <summary>
        ///     Cease routing all inputs associated with this provider to all
        ///     bound <see cref="Possessor"/>.
        /// </summary>
        public void UnbindAll()
        {
            _possessors.Remove(null);
            foreach (var possessor in _possessors)
            {
                possessor.OnUnboundFromInputProvider(this);
            }

            _possessors.Clear();
        }

        /// <summary>
        ///     Polls look input for this provider.
        /// </summary>
        public abstract Vector3 GetLookInput();

        internal void InvokeJump()
        {
            // This event cannot be invoked from inheriting types,
            // so we have to invoke this method instead. :(
            Jump?.Invoke();
        }
    }
}
