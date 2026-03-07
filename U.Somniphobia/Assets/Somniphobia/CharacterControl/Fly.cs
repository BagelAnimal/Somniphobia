using UnityEngine;

namespace FulcrumGames.CharacterControl
{
    /// <summary>
    ///     Gradually applies force to an object based on input. Enables flight
    ///     as long as the user is opting out of gravitation. Supports a
    ///     y-input in the movement input which will allow the user to move
    ///     vertically.
    /// </summary>
    public class Fly : MonoBehaviour
    {
        [SerializeField]
        private Transform _directionAnchor;

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private float _speed = 5.0f;

        [SerializeField]
        private float _speedInterpolant = 0.15f;

        private Vector3 _input = Vector3.zero;

        private void FixedUpdate()
        {
            if (!_directionAnchor)
            {
                Debug.LogError($"{name}'s {nameof(Fly)}" +
                    $"is missing a {nameof(_directionAnchor)}!", this);
                enabled = false;
                return;
            }

            if (!_rigidbody)
            {
                Debug.LogError($"{name}'s {nameof(Fly)}" +
                    $"is missing a {nameof(Rigidbody)}!", this);
                enabled = false;
                return;
            }

            _rigidbody.angularVelocity = Vector3.zero;
            var currentVelocity = _rigidbody.linearVelocity;

            var forward = _directionAnchor.forward;
            var right = _directionAnchor.right;
            var up = _directionAnchor.up;

            var desiredDirection = forward * _input.z + right * _input.x + up * _input.y;
            var desiredSpeed = _speed;
            var desiredVelocity = desiredDirection * desiredSpeed;

            var newVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, _speedInterpolant);

            var velocityDelta = newVelocity - currentVelocity;
            _rigidbody.AddForce(velocityDelta, ForceMode.VelocityChange);
        }

        public void SetInput(Vector3 input)
        {
            _input = input;
        }
    }
}
