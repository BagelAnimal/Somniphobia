using FulcrumGames.Somniphobia;
using UnityEngine;
using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace FulcrumGames.Possession
{
    /// <summary>
    ///     Represents a player in the game. A player delegates input actions
    ///     to some number of <see cref="Possessor"/>s in the world.
    /// </summary>
    public class Player : InputProvider
    {
        private const float MouseLookSensitivity = 1.0f;
        private const bool VerticalLookInverted = false;
        private const bool HorizontalLookInverted = false;

        private InputActions _inputActions;
        public InputActions InputActions => _inputActions;

        public void Initialize(string name)
        {
            if (_inputActions != null)
                return;

            SetName(name);

            _inputActions = new();
            _inputActions.Enable();
            _inputActions.World.Enable();
            _inputActions.World.Jump.performed += OnJumpInputProvided;
        }

        public void UpdateLookInput()
        {
            var rawInput = _inputActions.World.Look.ReadValue<Vector2>();

            var verticalLook = rawInput.y * MouseLookSensitivity;
            verticalLook = VerticalLookInverted ? verticalLook : -verticalLook;

            var horizontalLook = rawInput.x * MouseLookSensitivity;
            horizontalLook = HorizontalLookInverted ? -horizontalLook : horizontalLook;

            var processedInput = new Vector3(verticalLook, horizontalLook, 0.0f);
            _lookInput = processedInput;
        }

        private void OnJumpInputProvided(InputContext context)
        {
            JumpPressed();
        }

        public void Teardown()
        {
            if (_inputActions == null)
                return;

            UnbindAll();

            _inputActions.World.Jump.performed -= OnJumpInputProvided;
            _inputActions.World.Disable();
            _inputActions.Disable();
            _inputActions.Dispose();
            _inputActions = null;
        }
    }
}
