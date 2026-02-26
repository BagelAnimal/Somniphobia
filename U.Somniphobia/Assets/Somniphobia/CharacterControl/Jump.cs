using System;
using UnityEngine;

namespace FulcrumGames.CharacterControl
{
    /// <summary>
    ///     Applies force upward when executed.
    /// </summary>
    [DisallowMultipleComponent]
    public class Jump : MonoBehaviour
    {
        public event Action Jumped;

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private Crouch _crouch;

        [SerializeField]
        private GroundDetector _groundDetector;

        [SerializeField]
        private float _jumpHeight = 1.5f;

        [SerializeField]
        private bool _autoBhop = false;

        [SerializeField]
        [Tooltip("The number of frames jump will be used if an input is provided in mid-air.")]
        private int _jumpMemoryFrames = 6;

        [SerializeField]
        [Tooltip("The number of frames jump is available after becoming ungrounded.")]
        private int _coyoteTimeFrames = 6;

        [SerializeField]
        private int _jumpCooldownFrames = 8;

        [SerializeField]
        [Tooltip("The number of frames to remain ungrounded after starting a jump.")]
        private int _forceUngroundedFrames = 8;

        private Vector3 _gravity;

        private bool _input = false;
        private int _framesSinceLastInput = 0;
        private int _framesSinceLastJump = 0;
        private bool _isJumping = false;

        private void Awake()
        {
            _gravity = Physics.gravity;
        }

        private void FixedUpdate()
        {
            if (!_rigidbody)
            {
                Debug.LogError($"{name}'s {nameof(Jump)}" +
                    $"is missing a {nameof(Rigidbody)}!", this);
                enabled = false;
                return;
            }

            if (!_groundDetector)
            {
                Debug.LogError($"{name}'s {nameof(Jump)}" +
                    $"is missing a {nameof(GroundDetector)}!", this);
                enabled = false;
                return;
            }

            _framesSinceLastJump++;
            _framesSinceLastInput++;

            var gravityDirection = _gravity.normalized;
            var velocityAgainstGravity = -Vector3.Dot(_rigidbody.linearVelocity, gravityDirection);

            var reachedInflection = velocityAgainstGravity <= 0.0f;
            _isJumping = _isJumping && !reachedInflection && !_groundDetector.IsGrounded;

            if (!_input)
                return;

            if (_crouch && _crouch.IsCrouching && !_crouch.AllowJumpingWhileCrouching)
                return;

            var inputHasExpired = _framesSinceLastInput > _jumpMemoryFrames;
            if (inputHasExpired && !_autoBhop)
            {
                _input = false;
                return;
            }

            if (_isJumping)
                return;

            if (_framesSinceLastJump < _jumpCooldownFrames)
                return;

            var canCoyoteJump = _groundDetector.UngroundedFrames <= _coyoteTimeFrames;
            if (!_groundDetector.IsGrounded && !canCoyoteJump)
                return;

            _isJumping = true;
            _framesSinceLastJump = 0;

            // Velocity needed to reach height given gravity = sqrt(2gh).
            var g = _gravity.magnitude;
            var jumpMagnitude = Mathf.Sqrt(2.0f * g * _jumpHeight);
            var jumpVector = _rigidbody.mass * jumpMagnitude * -gravityDirection;
            _rigidbody.AddForce(jumpVector, ForceMode.Impulse);

            _input = _autoBhop;

            Jumped?.Invoke();
            _groundDetector.ForceUngroundedForFrames(_forceUngroundedFrames);
        }

        public void OnJumpPressed()
        {
            _input = true;
            _framesSinceLastInput = 0;
        }

        public void OnJumpReleased()
        {
            if (_autoBhop)
            {
                _input = false;
            }
        }

        public void SetGravity(Vector3 gravity)
        {
            _gravity = gravity;
        }
    }
}
