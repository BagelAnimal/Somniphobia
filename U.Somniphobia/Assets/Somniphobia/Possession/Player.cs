using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace FulcrumGames.Possession
{
    /// <summary>
    ///     Represents a player in the game. A player delegates input actions
    ///     to some number of <see cref="Possessor"/>s in the world.
    /// </summary>
    public class Player : InputProvider
    {
        private InputActions _inputActions;
        public InputActions InputActions => _inputActions;

        public void Initialize(string name)
        {
            SetName(name);

            _inputActions = new();
            _inputActions.Enable();
            _inputActions.World.Enable();
            _inputActions.World.Jump.performed += OnJumpInputProvided;
        }

        private void OnJumpInputProvided(InputContext context)
        {
            JumpPressed();
        }

        public void Teardown()
        {
            _inputActions.World.Jump.performed -= OnJumpInputProvided;
            _inputActions.World.Disable();
            _inputActions.Disable();
            _inputActions = null;
        }
    }
}
