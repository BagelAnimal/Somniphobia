using UnityEngine;

namespace FulcrumGames.CharacterControl
{
    /// <summary>
    ///     Rotates a target object given an input vector.
    /// </summary>
    [DisallowMultipleComponent]
    public class Look : MonoBehaviour
    {
        public float Smoothing = 0.0f;

        [SerializeField]
        private Transform _yawPivot;

        [SerializeField]
        private Transform _pitchPivot;

        [SerializeField]
        private Transform _rollPivot;

        [SerializeField]
        private float _maxPitch = 85.0f;

        [SerializeField]
        private float _minPitch = -110.0f;

        private Vector3 _input = Vector3.zero;

        private Vector3 _rotationVelocity = Vector3.zero;
        private Vector3 _targetRotationEuler = Vector3.zero;

        private float _pitch = 0.0f;
        private float _yaw = 0.0f;
        private float _roll = 0.0f;

        public Vector3 Forward => _rollPivot ? _rollPivot.transform.forward : transform.forward;

        private void Update()
        {
            if (!_yawPivot)
            {
                Debug.LogError($"{name}'s {nameof(Look)}" +
                    $"is missing a {nameof(_yawPivot)}!", this);
                enabled = false;
                return;
            }

            if (!_pitchPivot)
            {
                Debug.LogError($"{name}'s {nameof(Look)}" +
                    $"is missing a {nameof(_pitchPivot)}!", this);
                enabled = false;
                return;
            }

            if (!_rollPivot)
            {
                Debug.LogError($"{name}'s {nameof(Look)}" +
                    $"is missing a {nameof(_rollPivot)}!", this);
                enabled = false;
                return;
            }

            var currentRotation = new Vector3(_pitch, _yaw, _roll);
            var targetPitch = Mathf.Clamp(_targetRotationEuler.x + _input.x, _minPitch, _maxPitch);
            var targetYaw = _targetRotationEuler.y + _input.y;
            var targetRoll = _targetRotationEuler.z + _input.z;
            _targetRotationEuler = new Vector3(targetPitch, targetYaw, targetRoll);

            var smoothRotation = Vector3.SmoothDamp(currentRotation, _targetRotationEuler,
                ref _rotationVelocity, Smoothing);

            SetRotation(smoothRotation, clearSmoothing: false);
        }

        /// <summary>
        ///     Set the input to then be interpreted by the update loop.
        /// </summary>
        public void SetInput(Vector3 input)
        {
            _input = input;
        }

        /// <summary>
        ///     Updates the forward direction to look at the provided position.
        /// </summary>
        public void LookAt(Vector3 position)
        {
            var delta = position - _rollPivot.transform.position;
            if (delta.sqrMagnitude < Mathf.Epsilon)
                return;

            var forward = delta.normalized;
            SetForward(forward);
        }

        /// <summary>
        ///     Sets the forward direction of this object.
        /// </summary>
        public void SetForward(Vector3 forward)
        {
            var rotation = Quaternion.LookRotation(forward);
            var rotationEuler = rotation.eulerAngles;
            SetRotation(rotationEuler);
        }

        /// <summary>
        ///     Sets the rotation of this object.
        /// </summary>
        /// <param name="clearSmoothing">
        ///     Clears the target rotation that we're trying to smoothly transition to.
        /// </param>
        public void SetRotation(Quaternion rotation, bool clearSmoothing = true)
        {
            var rotationEuler = rotation.eulerAngles;
            SetRotation(rotationEuler, clearSmoothing);
        }

        /// <summary>
        ///     Copy the direction from another look instance to this instance.
        /// </summary>
        public void CopyRotationFrom(Look otherLook)
        {
            _pitch = otherLook._pitch;
            _yaw = otherLook._yaw;
            _roll = otherLook._roll;

            _rotationVelocity = Vector3.zero;
            _targetRotationEuler = new Vector3(_pitch, _yaw, _roll);

            _pitchPivot.transform.localRotation = Quaternion.Euler(Vector3.right * _pitch);
            _yawPivot.transform.localRotation = Quaternion.Euler(Vector3.up * _yaw);
            _rollPivot.transform.localRotation = Quaternion.Euler(Vector3.forward * _roll);
        }

        /// <summary>
        ///     Sets the rotation of this object.
        /// </summary>
        /// <param name="clearSmoothing">
        ///     Clears the target rotation that we're trying to smoothly transition to.
        /// </param>
        public void SetRotation(Vector3 rotationEuler, bool clearSmoothing = true)
        {
            _pitch = Mathf.Clamp(rotationEuler.x, _minPitch, _maxPitch);
            _yaw = rotationEuler.y;
            _roll = rotationEuler.z;

            if (clearSmoothing)
            {
                _targetRotationEuler = new Vector3(_pitch, _yaw, _roll);
            }

            _pitchPivot.transform.localRotation = Quaternion.Euler(Vector3.right * _pitch);
            _yawPivot.transform.localRotation = Quaternion.Euler(Vector3.up * _yaw);
            _rollPivot.transform.localRotation = Quaternion.Euler(Vector3.forward * _roll);
        }
    }
}
