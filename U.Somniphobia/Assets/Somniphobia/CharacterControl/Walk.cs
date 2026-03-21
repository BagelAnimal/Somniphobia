using UnityEngine;

namespace FulcrumGames.CharacterControl
{
    /// <summary>
    ///     Gradually applies force to an object based on input while enabling quake-style
    ///     strafing.
    /// </summary>
    [DisallowMultipleComponent]
    public class Walk : MonoBehaviour
    {
        [SerializeField]
        private Transform _directionAnchor;

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private GroundDetector _groundDetector;

        [SerializeField]
        private Crouch _crouch;

        [SerializeField]
        private float _groundSpeed = 5.0f;

        [SerializeField]
        private float _groundAcceleration = 50.0f;

        [SerializeField]
        private float _groundFriction = 20.0f;

        [SerializeField]
        private float _airSpeed = 0.5f;

        [SerializeField]
        private float _airAcceleration = 400.0f;

        [SerializeField]
        private float _airFriction = 20.0f;

        [SerializeField]
        private float _crouchSpeed = 2.5f;

        [SerializeField]
        private float _crouchAcceleration = 50.0f;

        [SerializeField]
        private float _crouchFriction = 20.0f;

        [SerializeField]
        [Tooltip("Extends airtime on touching the ground for a few seconds to enable b-hopping.")]
        private int _bhopWindowFrames = 4;

        [SerializeField]
        private float _minStepHeight = 0.05f;

        [SerializeField]
        private float _maxStepHeight = 0.4f;

        private bool _isWalking = false;
        public bool IsWalking => _isWalking;

        private float _currentSpeed = 0.0f;
        public float CurrentSpeed => _currentSpeed;

        private Vector3 _input = Vector3.zero;

        private void OnCollisionEnter(Collision collision)
        {
            HandleStepHeight(collision);
        }

        private void FixedUpdate()
        {
            if (!_directionAnchor)
            {
                Debug.LogError($"{name}'s {nameof(Walk)}" +
                    $"is missing a {nameof(_directionAnchor)}!", this);
                enabled = false;
                return;
            }

            if (!_rigidbody)
            {
                Debug.LogError($"{name}'s {nameof(Walk)}" +
                    $"is missing a {nameof(Rigidbody)}!", this);
                enabled = false;
                return;
            }

            if (!_groundDetector)
            {
                Debug.LogError($"{name}'s {nameof(Walk)}" +
                    $"is missing a {nameof(GroundDetector)}!", this);
                enabled = false;
                return;
            }

            _rigidbody.angularVelocity = Vector3.zero;
            var currentVelocity = _rigidbody.linearVelocity;
            var desiredVelocity = currentVelocity;

            var forward = _directionAnchor.forward;
            var right = _directionAnchor.right;

            _isWalking = _input.x != 0.0f || _input.z != 0.0f;
            var desiredDirection = forward * _input.z + right * _input.x;

            var isGrounded = _groundDetector.IsGrounded;
            var groundNormal = _groundDetector.GroundNormal;
            var groundedFrames = _groundDetector.GroundedFrames;

            // Project direction onto ground, scale normal by projection, and subtract from direction.
            var directionNormalMatch = Vector3.Dot(desiredDirection, groundNormal);
            var matchedNormal = groundNormal * directionNormalMatch;
            desiredDirection -= matchedNormal;

            var desiredSpeed = isGrounded ? _groundSpeed : _airSpeed;
            var friction = isGrounded ? _groundFriction : _airFriction;
            var acceleration = isGrounded ? _groundAcceleration : _airAcceleration;

            // TECH DEBT: Provided that further conditions scale these values,
            // we will want some sort of movement modifier stack that we can push
            // to and pop from so it's easier to add modifiers without introducing
            // new dependencies.
            if (isGrounded && _crouch)
            {
                var isCrouching = _crouch.IsCrouching;
                if (isCrouching)
                {
                    desiredSpeed = _crouchSpeed;
                    friction = _crouchFriction;
                    acceleration = _crouchAcceleration;
                }
            }

            // Apply friction if the character is on the ground.
            var useGroundMovement = isGrounded && groundedFrames >= _bhopWindowFrames;
            if (useGroundMovement)
            {
                var excessSpeed = _currentSpeed - _groundSpeed;

                // Amp up friction if we're moving too fast.
                if (excessSpeed > 0.5f)
                {
                    friction *= excessSpeed;
                }

                friction *= Time.fixedDeltaTime;
                var newSpeed = _currentSpeed - friction;
                newSpeed = Mathf.Max(newSpeed, 0.0f);

                if (newSpeed > float.Epsilon)
                {
                    var frictionScalar = newSpeed / _currentSpeed;
                    desiredVelocity = currentVelocity * frictionScalar;
                }
                else
                {
                    desiredVelocity *= newSpeed;
                }
            }

            // Lack of normalization and odd use of dot product to enable quake-style movement.
            var currentDirection = currentVelocity;
            var directionMatch = Vector3.Dot(currentDirection, desiredDirection);

            var speedChange = desiredSpeed - directionMatch;
            var minSpeedChange = 0.0f;
            var maxSpeedChange = acceleration * Time.fixedDeltaTime;
            speedChange = Mathf.Clamp(speedChange, minSpeedChange, maxSpeedChange);
            var addedVelocity = desiredDirection * speedChange;

            // Only clamp velocity if we're on the ground to enable air-strafing.
            if (isGrounded)
            {
                if (_currentSpeed < desiredSpeed)
                {
                    desiredVelocity += addedVelocity;
                }
            }
            else
            {
                desiredVelocity += addedVelocity;
            }

            var velocityDelta = desiredVelocity - currentVelocity;
            _rigidbody.AddForce(velocityDelta, ForceMode.VelocityChange);

            var up = _directionAnchor.transform.up;
            var velocityAlongUp = Vector3.Dot(_rigidbody.linearVelocity, up) * up;
            var flattenedVelocity = _rigidbody.linearVelocity - velocityAlongUp;
            _currentSpeed = flattenedVelocity.magnitude;
        }

        public void SetInput(Vector3 input)
        {
            _input = input;
        }

        private void HandleStepHeight(Collision collision)
        {
            if (!_groundDetector)
                return;

            if (!_groundDetector.IsGrounded)
                return;

            for (int i = 0; i < collision.contactCount; i++)
            {
                var contact = collision.contacts[0];

                var contactAngle = Vector3.Angle(transform.up, contact.normal);
                if (contactAngle <= _groundDetector.MaxGroundAngle)
                    continue;

                var contactPosition = contact.point;
                var contactRelativePosition = contactPosition - transform.position;
                var contactDirection = contactRelativePosition.normalized;
                var contactDistance = contactRelativePosition.magnitude;
                var stepCheckBump = contactDirection * (contactDistance + 0.1f);

                var checkOrigin = transform.position + stepCheckBump + (transform.up * _maxStepHeight);
                var checkDistance = _maxStepHeight;
                var checkDirection = -transform.up;
                var foundStep = Physics.Raycast(checkOrigin, checkDirection, out var stepHit, checkDistance);
                if (!foundStep)
                    continue;

                if (stepHit.distance <= 0.0f)
                    return;

                var stepPosition = stepHit.point;
                var stepHeight = stepPosition.y - transform.position.y;
                var offsetToApply = transform.up * (stepHeight + 0.01f) + contactDirection * 0.01f;
                var newPosition = _rigidbody.position + offsetToApply;
                _rigidbody.MovePosition(newPosition);

                return;
            }
        }
    }
}
