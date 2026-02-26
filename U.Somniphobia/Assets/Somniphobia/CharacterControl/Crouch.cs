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
        [Range(0.1f, 1.0f)]
        private float _crouchHeight = 0.6f;

        [SerializeField]
        [Range(0.01f, 2.0f)]
        private float _crouchTime = 0.3f;

        [SerializeField]
        private AnimationCurve _crouchCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

        [SerializeField]
        private bool _allowJumpingWhileCrouching = false;
        public bool AllowJumpingWhileCrouching => _allowJumpingWhileCrouching;

        private float _defaultHeight = 0.0f;
        private float _crouchAmount = 0.0f;
        private bool _input = false;
        public bool IsCrouching => _crouchAmount > 0.5f;

        private void Awake()
        {
            if (!_collider)
            {
                Debug.LogError($"{name}'s {nameof(Crouch)}" +
                    $"is missing a {nameof(BoxCollider)}!", this);
                enabled = false;
                return;
            }

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

            if (_crouchTime <= 0)
            {
                Debug.LogError($"{name}'s {nameof(Crouch)}" +
                    $" was given a crouch time of {_crouchTime}!", this);
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
                var checkOrigin = transform.position;
                var originOffset = _groundDetector.IsGrounded ? _defaultHeight : _defaultHeight * _crouchHeight;
                checkOrigin += 0.5f * originOffset * transform.up;
                var checkExtents = new Vector3(_collider.size.x, _defaultHeight, _collider.size.z) * 0.49f;

                var overlaps = Physics.OverlapBox(checkOrigin,
                    checkExtents, transform.rotation, _obstacleLayers);

                // Only early return out of the method if the overlap is not our game object.
                for (int i = 0; i < overlaps.Length; i++)
                {
                    var overlap = overlaps[i];
                    if (overlap.gameObject == gameObject)
                        continue;

                    if (overlap.isTrigger)
                        continue;

                    return;
                }
            }

            // Update crouch state gradually if grounded, or instantly if ungrounded.
            // Instant extent modification in mid-air is meant to enable a more responsive
            // crouch jump.
            if (isEnteringCrouch)
            {
                _crouchAmount += Time.deltaTime / _crouchTime;
                _crouchAmount = Mathf.Min(_crouchAmount, 1.0f);
                _crouchAmount = _groundDetector.IsGrounded ? _crouchAmount : 1.0f;
            }
            else
            {
                _crouchAmount -= Time.deltaTime / _crouchTime;
                _crouchAmount = Mathf.Max(_crouchAmount, 0.0f);
                _crouchAmount = _groundDetector.IsGrounded ? _crouchAmount : 0.0f;
            }

            float crouchT = _crouchCurve.Evaluate(_crouchAmount);
            float sizeY = Mathf.Lerp(_defaultHeight, _defaultHeight * _crouchHeight, crouchT);
            _collider.size = new(_collider.size.x, sizeY, _collider.size.z);
            _collider.center = new(_collider.center.x, sizeY / 2.0f, _collider.center.z);

            if (!_groundDetector.IsGrounded)
            {
                // If we're transitioning in mid-air, move the origin upward to enable crouch-jumping.
                var bumpHeight = _defaultHeight - sizeY;
                bumpHeight = isExitingCrouch ? -(_defaultHeight - (_defaultHeight * _crouchHeight)) : bumpHeight;
                var bumpAmount = new Vector3(0.0f, bumpHeight, 0.0f);
                var newPosition = transform.position + bumpAmount;
                _rigidbody.MovePosition(newPosition);
            }

            // If a character or perspective point is attached to our character, we need to
            // move it vertically to account for the change in height. This script is and
            // should continue to be unaware of how this is done, so it has no opinion on
            // this. Ideally, some script should run in the camera's anchor point that
            // updates the camera's relative position based on the character's collider height.
        }

        public void SetInput(bool input)
        {
            _input = input;
        }
    }
}
