using UnityEngine;

namespace FulcrumGames.CharacterControl
{
    /// <summary>
    ///     Gradually applies force to an object based on input while enable quake-style
    ///     strafing. Works best when combined with a jump script, and decides how much
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(GroundDetector))]
    public class Walk : MonoBehaviour
    {
        [SerializeField]
        private Transform _directionAnchor;

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private GroundDetector _groundDetector;

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
        [Tooltip("Extends airtime on touching the ground for a few seconds to enable b-hopping.")]
        private int _bhopWindowFrames = 4;

        private bool _isWalking = false;
        public bool IsWalking => _isWalking;

        private float _currentSpeed = 0.0f;
        public float CurrentSpeed => _currentSpeed;

        private Vector3 _input = Vector3.zero;

        private void Awake()
        {
            if (!_rigidbody)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }

            if (!_groundDetector)
            {
                _groundDetector = GetComponent<GroundDetector>();
            }

            if (!_directionAnchor)
            {
                _directionAnchor = transform;
            }
        }

        private void FixedUpdate()
        {
            if (!_rigidbody)
                return;

            if (!_groundDetector)
                return;

            var currentVelocity = _rigidbody.linearVelocity;
            var desiredVelocity = currentVelocity;

            var forward = _directionAnchor.forward;
            var right = _directionAnchor.right;

            _isWalking = _input != Vector3.zero;
            var desiredDirection = forward * _input.y + right * _input.x;
            desiredDirection = desiredDirection.XOZ();

            desiredDirection = new Vector3(desiredDirection.x, 0.0f, desiredDirection.z);
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
            _currentSpeed = desiredVelocity.XOZ().magnitude;
        }

        public void SetInput(Vector3 input)
        {
            _input = input;
        }
    }
}
