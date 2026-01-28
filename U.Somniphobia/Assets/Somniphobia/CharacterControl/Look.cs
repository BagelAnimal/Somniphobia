using UnityEngine;

namespace FulcrumGames.CharacterControl
{
    /// <summary>
    ///     Rotates a target object given an input vector.
    /// </summary>
    public class Look : MonoBehaviour
    {
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

        [SerializeField]
        private float _smoothing = 0.05f;

        private Vector3 _rotationVelocity = Vector3.zero;
        private Vector3 _targetRotationEuler = Vector3.zero;

        private float _pitch = 0.0f;
        private float _yaw = 0.0f;
        private float _roll = 0.0f;

        public Quaternion Rotation => Quaternion.Euler(_pitch, _yaw, _roll);
        public Vector3 Forward => Rotation * _rollPivot.transform.forward;

        /// <summary>
        ///     Updates the orientation of this object given some input vector.
        /// </summary>
        public void UpdateLook(Vector3 input)
        {
            var currentRotation = new Vector3(_pitch, _yaw, _roll);
            var targetPitch = Mathf.Clamp(_targetRotationEuler.x + input.x, _minPitch, _maxPitch);
            var targetYaw = _targetRotationEuler.y + input.y;
            var targetRoll = _targetRotationEuler.z + input.z;
            _targetRotationEuler = new Vector3(targetPitch, targetYaw, targetRoll);

            var smoothRotation = Vector3.SmoothDamp(currentRotation, _targetRotationEuler,
                ref _rotationVelocity, _smoothing);

            SetRotation(smoothRotation, clearSmoothing: false);
        }

        /// <summary>
        ///     Updates the forward direction to look at the provided position.
        /// </summary>
        public void LookAt(Vector3 position)
        {
            var delta = position - _rollPivot.transform.position;
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
