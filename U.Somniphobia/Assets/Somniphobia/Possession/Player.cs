using System.Collections.Generic;
using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace FulcrumGames.Possession
{
    /// <summary>
    ///     Represents a player in the game. A player delegates input actions
    ///     to some number of <see cref="Possessor"/>s in the world.
    /// </summary>
    public class Player
    {
        private string _name = "Player";
        public string Name => _name;

        private InputActions _inputActions;
        public InputActions InputActions => _inputActions;

        private List<Possessor> _possessors = new();
        public IReadOnlyList<Possessor> Possessors => _possessors;

        public void Initialize(string name)
        {
            _name = name;

            _inputActions = new();
            _inputActions.World.Enable();
            _inputActions.World.Jump.performed += JumpPressed;
        }

        public void Teardown()
        {
            _inputActions.World.Jump.performed -= JumpPressed;
        }

        public void BindPossessor(Possessor possessor)
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

        public void UnbindPossessor(Possessor possessor)
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

        public void JumpPressed(InputContext context)
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
