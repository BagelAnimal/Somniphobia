using System;
using UnityEngine;

namespace FulcrumGames.Possession
{
    /// <summary>
    ///     A player is used to represent the user of this application.
    ///     It serves to receive and delegate their intent and their preferences.
    /// </summary>
    public class Player : MonoBehaviour
    {
        /// <summary>
        ///     Raised when an input is provided. Subscribers can filter by
        ///     input type and state to decide what they specifically want
        ///     to respond to.
        /// </summary>
        public event Action<InputType, InputState> InputProvided;

        [SerializeField]
        private Camera _camera;

        [SerializeField]
        private float _mouseLookSensitivity = 0.2f;
        public float MouseLookSensitivity => _mouseLookSensitivity;

        [SerializeField]
        private bool _verticalLookInverted = false;
        public bool VerticalLookInverted => _verticalLookInverted;

        [SerializeField]
        private bool _horizontalLookInverted = false;
        public bool HorizontalLookInverted => _horizontalLookInverted;

        [SerializeField]
        private float _fieldOfView = 90.0f;
        public float FieldOfView => _fieldOfView;

        private InputActions _inputActions;
        public InputActions InputActions => _inputActions;

        private bool _isInitialized = false;

        private void Awake()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            if (_inputActions == null)
                return;

            _inputActions.World.Disable();
            _inputActions.Disable();
            _inputActions.Dispose();
            _inputActions = null;
        }

        public Vector3 GetLookInput()
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

        public Vector3 GetMoveInput()
        {
            if (_inputActions == null)
                return default;

            var rawInput = _inputActions.World.Move.ReadValue<Vector2>();
            var verticalInput = 0.0f;
            verticalInput = _inputActions.World.Jump.IsPressed() ? verticalInput + 1.0f : verticalInput;
            verticalInput = _inputActions.World.Crouch.IsPressed() ? verticalInput - 1.0f : verticalInput;

            var inputVector3 = new Vector3(rawInput.x, verticalInput, rawInput.y);
            return inputVector3;
        }

        public void EnableWorldControls()
        {
            if (!_isInitialized)
                return;

            _inputActions.World.Enable();
        }

        private void Initialize()
        {
            if (_isInitialized)
                return;

            if (_camera)
            {
                _camera.fieldOfView = _fieldOfView;
            }

            _isInitialized = true;

            _inputActions = new InputActions();

            _inputActions.World.Jump.performed += (_) 
                => InputProvided?.Invoke(InputType.Jump, InputState.Pressed);
            _inputActions.World.Jump.canceled += (_) 
                => InputProvided?.Invoke(InputType.Jump, InputState.Released);

            _inputActions.World.Crouch.performed += (_) 
                => InputProvided?.Invoke(InputType.Crouch, InputState.Pressed);
            _inputActions.World.Crouch.canceled += (_) 
                => InputProvided?.Invoke(InputType.Crouch, InputState.Released);

            _inputActions.World.Possess.performed += (_)
                => InputProvided?.Invoke(InputType.Possess, InputState.Pressed);
            _inputActions.World.Possess.canceled += (_)
                => InputProvided?.Invoke(InputType.Possess, InputState.Released);
        }
    }
}
