using UnityEngine;
using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace FulcrumGames.Possession
{
    /// <summary>
    ///     Represents a player in the game. A player delegates input actions
    ///     to some number of possessors in the world.
    /// </summary>
    [DisallowMultipleComponent]
    public class Player : InputProvider
    {
        private const float MouseLookSensitivity = 0.2f;
        private const bool VerticalLookInverted = false;
        private const bool HorizontalLookInverted = false;
        private const float FieldOfView = 90.0f;

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
            _inputActions.World.Jump.canceled += OnJumpInputProvided;
            _inputActions.World.Crouch.performed += OnCrouchInputProvided;
            _inputActions.World.Crouch.canceled += OnCrouchInputProvided;
        }

        public override Vector3 GetLookInput()
        {
            if (_inputActions == null)
                return default;

            var rawInput = _inputActions.World.Look.ReadValue<Vector2>();

            var verticalLook = rawInput.y * MouseLookSensitivity;
            verticalLook = VerticalLookInverted ? verticalLook : -verticalLook;

            var horizontalLook = rawInput.x * MouseLookSensitivity;
            horizontalLook = HorizontalLookInverted ? -horizontalLook : horizontalLook;

            var processedInput = new Vector3(verticalLook, horizontalLook, 0.0f);
            return processedInput;
        }

        public override Vector3 GetMoveInput()
        {
            if (_inputActions == null)
                return default;

            var rawInput = _inputActions.World.Move.ReadValue<Vector2>();
            var inputVector3 = new Vector3(rawInput.x, 0.0f, rawInput.y);
            return inputVector3;
        }

        protected override void OnPossessorBound(Possessor possessor)
        {
            var cameras = possessor.GetComponentsInChildren<Camera>();
            if (cameras.Length == 0)
                return;

            foreach (var camera in cameras)
            {
                camera.fieldOfView = FieldOfView;
            }
        }

        private void OnJumpInputProvided(InputContext context)
        {
            InvokeJump(context);
        }

        private void OnCrouchInputProvided(InputContext context)
        {
            InvokeCrouch(context);
        }

        public void Teardown()
        {
            if (_inputActions == null)
                return;

            UnbindAll();

            _inputActions.World.Jump.performed -= OnJumpInputProvided;
            _inputActions.World.Jump.canceled -= OnJumpInputProvided;
            _inputActions.World.Crouch.performed -= OnCrouchInputProvided;
            _inputActions.World.Crouch.canceled -= OnCrouchInputProvided;
            _inputActions.World.Disable();
            _inputActions.Disable();
            _inputActions.Dispose();
            _inputActions = null;
        }
    }
}
