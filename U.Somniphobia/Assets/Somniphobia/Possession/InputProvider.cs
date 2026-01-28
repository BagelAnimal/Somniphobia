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
        private string _name = "CPU";
        public string Name => _name;

        protected readonly HashSet<Possessor> _possessors = new();

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
        public void BindToPossessor(Possessor toBindTo)
        {
            _possessors.Remove(null);
            if (!toBindTo)
            {
                UnityEngine.Debug.LogWarning($"{_name} was given a null possessor to bind to!");
                return;
            }

            if (!_possessors.Add(toBindTo))
                return;

            toBindTo.OnBoundToInputProvider(this);
        }

        /// <summary>
        ///     Cease routing all inputs associated with this provider to the
        ///     <see cref="Possessor"/>.
        /// </summary>
        public void UnbindFromPossessor(Possessor toUnbindFrom)
        {
            _possessors.Remove(null);
            if (!toUnbindFrom)
            {
                UnityEngine.Debug.LogWarning($"{_name} was given a null possessor to unbind" +
                    $"from!");
                return;
            }

            if (!_possessors.Remove(toUnbindFrom))
                return;

            toUnbindFrom.OnUnboundFromInputProvider(this);
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

        public abstract Vector3 GetLookInput();

        internal void JumpPressed()
        {
            _possessors.Remove(null);
            foreach (var possessor in _possessors)
            {
                possessor.OnJumpPressed();
            }
        }
    }
}
