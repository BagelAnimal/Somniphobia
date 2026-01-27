using System.Collections.Generic;
using System.Linq;
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
        private readonly HashSet<Possessor> _possessors = new();
        private bool _isBeingDestroyed = false;

        public bool IsPossessed => _possessors.Count > 0;
        public Possessor CurrentPossessor => IsPossessed ? _possessors.First() : null;

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

            _possessors.Add(possessor);
        }

        /// <summary>
        ///     Call when removing a <see cref="Possessor"/> from a given possessable.
        ///     Lets the possessable track it, thus helping it avoid a case where it
        ///     tries to let an uncaring or null <see cref="Possessor"/> know that
        ///     it is being destroyed.
        /// </summary>
        public void OnUnpossessedBy(Possessor possessor)
        {
            // When this is in the process of being destroyed, avoid modifying the
            // possessors collection since we're iterating over it.
            if (_isBeingDestroyed)
                return;

            _possessors.Remove(null);
            if (!possessor)
            {
                Debug.LogWarning($"{name} was given a null possessor to be unpossessed from!",
                    this);
                return;
            }

            _possessors.Remove(possessor);
        }

        /// <summary>
        ///     Mutation of game state reflecting a jump action on a possessable entity.
        /// </summary>
        public void OnJumpPressed()
        {
            Debug.Log("Jumped!");
        }
    }
}
