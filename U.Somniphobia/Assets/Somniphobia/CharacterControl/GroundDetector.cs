using System;
using UnityEngine;

namespace FulcrumGames.CharacterControl
{
    /// <summary>
    ///     Checks using this object's down for objects at angles that can be considered
    ///     ground. Fires rays down in a cross shape whose size is determined by the
    ///     provided collider, and maintains state representing whether valid ground is
    ///     below and what the normal of that ground is.
    /// </summary>
    [DisallowMultipleComponent]
    public class GroundDetector : MonoBehaviour
    {
        private const float CheckHeightBump = 0.5f;

        public event Action Grounded;
        public event Action Ungrounded;

        [SerializeField]
        private Collider _collider;

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        [Tooltip("Layers to be completely ignored")]
        private LayerMask _ignoredLayers = 0;

        [SerializeField]
        [Tooltip("Layers that should never be considered ground, but that can contribute to slip")]
        private LayerMask _slipLayers = 0;

        [SerializeField]
        private float _maxGroundAngle = 35.0f;

        [SerializeField]
        private float _groundCheckDistance = 0.05f;

        [SerializeField]
        [Tooltip("The speed threshhold at which the object cannot become grounded.")]
        private float _maxGroundedSpeed = 20.0f;

        [SerializeField]
        private bool _drawDebugLines = false;

        private bool _isGrounded = true;
        /// <summary>
        ///     Whether this object is currently on valid ground.
        /// </summary>
        public bool IsGrounded => _isGrounded;

        private int _groundedFrames = 0;
        /// <summary>
        ///     For how many frames this object has been on valid ground.
        /// </summary>
        public int GroundedFrames => _groundedFrames;

        private int _ungroundedFrames = 0;
        /// <summary>
        ///     For how many frames this object has been off valid ground.
        /// </summary>
        public int UngroundedFrames => _ungroundedFrames;

        private float _groundAngle = 0.0f;
        /// <summary>
        ///     The angle of the ground detected beneath this object in degrees.
        /// </summary>
        public float GroundAngle => _groundAngle;

        private Vector3 _groundNormal = Vector3.zero;
        /// <summary>
        ///     The normal of the ground detected beneath this object.
        /// </summary>
        public Vector3 GroundNormal => _groundNormal;

        private int _ungroundFramesRemaining = 0;

        private void FixedUpdate()
        {
            if (!_collider)
            {
                Debug.LogError($"{name}'s {nameof(GroundDetector)}" +
                    $"is missing a {nameof(Collider)}!", this);
                return;
            }

            if (!_rigidbody)
            {
                Debug.LogError($"{name}'s {nameof(GroundDetector)}" +
                    $"is missing a {nameof(Rigidbody)}!", this);
                return;
            }

            var wasGrounded = _isGrounded;

            // We will raycast down in the following shape...
            // x * x * x
            // * * * * *
            // x * x * x
            // * * * * *
            // x * x * x
            // First we grab the center, and add bump it upward a bit.
            var checkOrigin = transform.position + transform.up * CheckHeightBump;
            // Then, we get the distance that each point on the shape should be from the center.
            // We multiply it down a bit to avoid reading being pressed against a wall as grounded.
            var checkOffset = _collider.bounds.extents.x * 0.9f;

            var groundHitCount = 0;
            var groundHitNormalSum = Vector3.zero;
            for (int i = 0; i < 9; i++)
            {
                var origin = checkOrigin;
                switch (i)
                {
                    case 0:
                        break;
                    case 1:
                        origin += transform.forward * checkOffset;
                        break;
                    case 2:
                        origin += -transform.forward * checkOffset;
                        break;
                    case 3:
                        origin += transform.right * checkOffset;
                        break;
                    case 4:
                        origin += -transform.right * checkOffset;
                        break;
                    case 5:
                        origin += (transform.right + transform.forward) * checkOffset;
                        break;
                    case 6:
                        origin += (-transform.right + transform.forward) * checkOffset;
                        break;
                    case 7:
                        origin += (transform.right + -transform.forward) * checkOffset;
                        break;
                    case 8:
                        origin += (-transform.right + -transform.forward) * checkOffset;
                        break;
                    default:
                        Debug.LogError($"{gameObject.name}'s ground check is evil!");
                        break;
                }

                // Extend the raycast distance a bit to account for the bump performed above.
                var distance = _groundCheckDistance + CheckHeightBump;
                var direction = -transform.up;
                var layers = ~_ignoredLayers;

                if (Physics.Raycast(origin, direction, out RaycastHit groundHit, distance, layers))
                {
                    // Ensure that we don't consider ourselves a source of ground.
                    var collider = groundHit.collider;
                    if (collider == _collider)
                        continue;

                    var layer = groundHit.collider.gameObject.layer;
                    var isSlipLayer = (_slipLayers & (1 << layer)) > 0;
                    if (isSlipLayer)
                        continue;

                    var groundAngle = Vector3.Angle(transform.up, groundHit.normal);
                    if (groundAngle > _maxGroundAngle)
                        continue;

                    groundHitCount++;
                    groundHitNormalSum += groundHit.normal;
                }

                if (_drawDebugLines)
                {
                    var end = origin + direction * distance;
                    Debug.DrawLine(origin, end, Color.yellow);
                }

                // If our first ray, the origin, finds the ground, just exit since the ground
                // angle being derived from the character's center is most intuitive.
                if (i == 0 && groundHitCount > 0)
                    break;
            }

            var speedSquared = _rigidbody.linearVelocity.sqrMagnitude;
            var maxSpeedSquared = _maxGroundedSpeed * _maxGroundedSpeed;
            var isBeyondMaxSpeed = speedSquared > maxSpeedSquared;

            _isGrounded = groundHitCount > 0 && !isBeyondMaxSpeed && _ungroundFramesRemaining <= 0;
            if (!_isGrounded)
            {
                _groundedFrames = 0;
                _ungroundedFrames++;

                if (_ungroundFramesRemaining > 0)
                {
                    _ungroundFramesRemaining--;
                }

                _groundAngle = 0.0f;
                _groundNormal = Vector3.zero;

                if (wasGrounded)
                {
                    Ungrounded?.Invoke();
                }

                return;
            }

            _ungroundedFrames = 0;
            _groundedFrames++;

            _groundNormal = groundHitNormalSum.normalized;
            _groundAngle = Vector3.Angle(transform.up, _groundNormal);

            if (!wasGrounded)
            {
                Grounded?.Invoke();
            }
        }

        public void ForceUngroundedForFrames(int frames)
        {
            _ungroundFramesRemaining = frames;
        }
    }
}
