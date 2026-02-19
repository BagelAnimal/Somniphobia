using FulcrumGames.CharacterControl;
using FulcrumGames.Possession;
using UnityEngine;

using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace FulcrumGames.Glue
{
    /// <summary>
    ///     Binds input provided by input providers to behaviors like walk and jump
    ///     to reduce interdependency in the project.
    ///     
    ///     Very bespoke, not intended to be reused in other projects.
    /// </summary>
    public class PossessorToCharacterControl : MonoBehaviour
    {
        [SerializeField]
        private Possessable _possessable;

        [SerializeField]
        private Jump _jump;

        [SerializeField]
        private Crouch _crouch;

        [SerializeField]
        private Walk _walk;

        [SerializeField]
        private Look _look;

        private void Awake()
        {
            if (_possessable)
            {
                _possessable.Jump += OnJumpInput;
                _possessable.Crouch += OnCrouchInput;
            }
        }

        private void OnDestroy()
        {
            if (_possessable)
            {
                _possessable.Jump -= OnJumpInput;
                _possessable.Crouch -= OnCrouchInput;
            }
        }

        private void Update()
        {
            if (!_possessable)
            {
                Debug.LogError($"{name}'s {nameof(PossessorToCharacterControl)}" +
                    $"is missing a {nameof(Possessable)}!", this);
                enabled = false;
                return;
            }

            if (_look)
            {
                var lookInput = _possessable.GetLookInput();
                _look.SetInput(lookInput);
            }

            if (_walk)
            {
                var walkInput = _possessable.GetMoveInput();
                _walk.SetInput(walkInput);
            }
        }

        private void OnJumpInput(InputContext inputContext)
        {
            if (!_jump)
                return;

            if (inputContext.performed)
            {
                _jump.OnJumpPressed();
            }
            else if (inputContext.canceled)
            {
                _jump.OnJumpReleased();
            }
        }

        private void OnCrouchInput(InputContext inputContext)
        {
            if (!_crouch)
                return;

            if (inputContext.performed)
            {
                _crouch.SetInput(true);
            }
            else if (inputContext.canceled)
            {
                _crouch.SetInput(false);
            }
        }
    }
}
