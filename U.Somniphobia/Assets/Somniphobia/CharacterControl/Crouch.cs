using UnityEngine;

namespace FulcrumGames.CharacterControl
{
    /// <summary>
    ///     Enables a gradual crouch and uncrouch given input for a given object.
    ///     Operates using a collider and a ground detector for state referencing.
    ///     Seeks to enable crouch-jumping both on the ground and in the air.
    /// </summary>
    [DisallowMultipleComponent]
    public class Crouch : MonoBehaviour
    {
        // We get specific about the collider type that we're looking for because it's
        // difficult to modify the bounds of the abstract type. Adding support for other
        // collider types should be pretty easy if we want that.
        [SerializeField]
        private BoxCollider _collider;

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private GroundDetector _groundDetector;

        [SerializeField]
        private LayerMask _obstacleLayers = 1;

        [SerializeField]
        private float _crouchHeight = 0.6f;

        [SerializeField]
        private float _crouchTime = 0.3f;

        [SerializeField]
        private AnimationCurve _crouchCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

        [SerializeField]
        private float _speedScalar = 0.5f;
        public float SpeedScalar => _speedScalar;

        [SerializeField]
        private float _accelerationScalar = 0.5f;
        public float AccelerationScalar => _accelerationScalar;

        [SerializeField]
        private float _frictionScalar = 1.0f;
        public float FrictionScalar => _frictionScalar;

        [SerializeField]
        private bool _allowJumpingWhileCrouching = false;
        public bool AllowJumpingWhileCrouching => _allowJumpingWhileCrouching;

        [SerializeField]
        private bool _enableDebugDrawing = false;

        private float _defaultHeight = 0.0f;
        private float _crouchAmount = 0.0f;
        private bool _input = false;
        public bool IsCrouching => _crouchAmount > 0.5f;

        private void Awake()
        {
            // TECH DEBT: Growing/shrinking is an intended mechanic. How do we handle this here?
            _defaultHeight = _collider.bounds.extents.y * 2.0f;
        }

        private void FixedUpdate()
        {
            if (!_collider)
            {
                Debug.LogError($"{name}'s {nameof(Crouch)}" +
                    $"is missing a {nameof(BoxCollider)}!", this);
                enabled = false;
                return;
            }

            if (!_rigidbody)
            {
                Debug.LogError($"{name}'s {nameof(Crouch)}" +
                    $"is missing a {nameof(Rigidbody)}!", this);
                enabled = false;
                return;
            }

            if (!_groundDetector)
            {
                Debug.LogError($"{name}'s {nameof(Crouch)}" +
                    $"is missing a {nameof(GroundDetector)}!", this);
                enabled = false;
                return;
            }

            var isEnteringCrouch = _input && _crouchAmount < 1.0f;
            var isExitingCrouch = !_input && _crouchAmount > 0.0f;
            if (!isEnteringCrouch && !isExitingCrouch)
                return;

            var crouchHeight = _defaultHeight * _crouchHeight;
            if (isExitingCrouch)
            {
                // If we're uncrouching in mid-air, feet are trying to go down. Otherwise, head
                // is trying to go up. We think of it this way to enable crouch-jumping.
                var checkDirection = _groundDetector.IsGrounded ? transform.up : -transform.up;
                var checkOrigin = transform.position + (_collider.size.y * 0.5f * transform.up);
                var checkExtents = new Vector3(_collider.size.x,
                    _defaultHeight * 0.5f, _collider.size.z) * 0.5f;

                var overlaps = Physics.OverlapBox(checkOrigin, checkExtents,
                    transform.rotation, _obstacleLayers);

                if (_enableDebugDrawing)
                {
                    Debug.DrawLine(checkOrigin, checkOrigin + checkExtents.y
                        * 2.0f * checkDirection, Color.green);
                }

                // Only early return out of the method if the overlap is not our game object.
                for (int i = 0; i < overlaps.Length; i++)
                {
                    var overlap = overlaps[i];
                    if (overlap.gameObject == gameObject)
                        continue;

                    return;
                }
            }

            // Update crouch state gradually if grounded, or instantly if ungrounded.
            // Instant extent modification in mid-air is meant to enable a more responsive
            // crouch jump.
            if (isEnteringCrouch)
            {
                _crouchAmount += Time.fixedDeltaTime / _crouchTime;
                _crouchAmount = Mathf.Min(_crouchAmount, 1.0f);
                _crouchAmount = _groundDetector.IsGrounded ? _crouchAmount : 1.0f;

            }
            else
            {
                _crouchAmount -= Time.fixedDeltaTime / _crouchTime;
                _crouchAmount = Mathf.Max(_crouchAmount, 0.0f);
                _crouchAmount = _groundDetector.IsGrounded ? _crouchAmount : 0.0f;
            }

            float crouchT = _crouchCurve.Evaluate(_crouchAmount);
            float sizeY = Mathf.Lerp(_defaultHeight, _defaultHeight * _crouchHeight, crouchT);
            _collider.size = new(_collider.size.x, sizeY, _collider.size.z);
            _collider.center = new(_collider.center.x, sizeY / 2.0f, _collider.center.z);

            // TODO: We need some way to move the pivot of the camera down.
            // If we were to bump the bottom of the collider up and then push the position
            // of the object down that would create this behavior, but that would result in
            // underground character origins which I don't think we want.
            // _possessable.SetPitchPivotHeight(_possessable.DefaultPitchPivotHeight * (sizeY / _defaultHeight));
            // _possessable._pitchPivot.localPosition = new(_soulAnchor.localPosition.x, height, _soulAnchor.localPosition.z);

            if (!_groundDetector.IsGrounded)
            {
                // If we're transitioning in mid-air, move the origin upward to enable crouch-jumping.
                var bumpHeight = _defaultHeight - sizeY;
                bumpHeight = isExitingCrouch ? -(_defaultHeight - (_defaultHeight * _crouchHeight)) : bumpHeight;
                var bumpAmount = new Vector3(0.0f, bumpHeight, 0.0f);
                var newPosition = transform.position + bumpAmount;
                _rigidbody.MovePosition(newPosition);
            }
        }

        public void SetInput(bool input)
        {
            _input = input;
        }
    }
}
