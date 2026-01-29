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
    [RequireComponent(typeof(Collider))]
    public class GroundDetector : MonoBehaviour
    {
        public event Action Grounded;
        public event Action Ungrounded;

        [SerializeField]
        private Collider _collider;

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

        private bool _isGrounded = true;
        /// <summary>
        ///     Whether this object is currently on valid ground.
        /// </summary>
        public bool IsGrounded => _isGrounded;

        private int _groundedSteps = 0;
        /// <summary>
        ///     For how many frames this object has been on valid ground.
        /// </summary>
        public int GroundedSteps => _groundedSteps;

        private int _ungroundedSteps = 0;
        /// <summary>
        ///     For how many frames this object has been off valid ground.
        /// </summary>
        public int UngroundedSteps => _ungroundedSteps;

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

        private void FixedUpdate()
        {
            if (!_collider)
                return;

            var wasGrounded = _isGrounded;

            // We will raycast down in a cross shape.
            // First we grab the center, and add bump it upward a bit.
            var checkOrigin = transform.position + transform.up * float.Epsilon;
            // Then, we get the distance that each point on the cross should be from the center.
            // We multiply it down a bit to avoid reading being pressed against a wall as grounded.
            var checkOffset = _collider.bounds.extents.x * 0.9f;

            var groundHitCount = 0;
            var groundHitNormalSum = Vector3.zero;
            for (int i = 0; i < 5; i++)
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
                    default:
                        Debug.LogError($"{gameObject.name}'s ground check is evil!");
                        break;
                }

                // Extend the raycast distance a bit to account for the bump performed above.
                var distance = _groundCheckDistance * (1.0f + float.Epsilon);
                var direction = -transform.up;
                var layers = ~_ignoredLayers;

                if (Physics.Raycast(origin, direction, out RaycastHit groundHit, distance, layers))
                {
                    var layer = groundHit.collider.gameObject.layer;
                    var isSlipLayer = (_slipLayers & layer) != 0;
                    if (isSlipLayer)
                        continue;

                    var groundAngle = Vector3.Angle(transform.up, groundHit.normal);
                    if (groundAngle > _maxGroundAngle)
                        continue;

                    groundHitCount++;
                    groundHitNormalSum += groundHit.normal;
                }
            }

            _isGrounded = groundHitCount > 0;
            if (!_isGrounded)
            {
                _groundedSteps = 0;
                _ungroundedSteps++;

                _groundAngle = 0.0f;
                _groundNormal = Vector3.zero;

                if (wasGrounded)
                {
                    Ungrounded?.Invoke();
                }

                return;
            }

            _ungroundedSteps = 0;
            _groundedSteps++;

            _groundNormal = groundHitNormalSum.normalized;
            _groundAngle = Vector3.Angle(transform.up, _groundNormal);

            if (!wasGrounded)
            {
                Grounded?.Invoke();
            }
        }
    }
}
