using UnityEngine;

namespace FulcrumGames.Possession
{
    public class RotateWithInput : MonoBehaviour
    {
        [SerializeField]
        private Transform _yawPivot;

        [SerializeField]
        private Transform _pitchPivot;

        [SerializeField]
        private Transform _rollPivot;

        [SerializeField]
        private Possessable _possessable;

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

        private void Update()
        {
            if (!_possessable)
                return;

            if (!_possessable.IsPossessed)
                return;

            var possessor = _possessable.CurrentPossessor;
            if (!possessor)
                return;

            if (!possessor.HasInputProvider)
                return;

            var inputProvider = possessor.CurrentInputProvider;
            if (inputProvider == null)
                return;

            var lookInput = inputProvider.GetLookInput();

            var currentRotation = new Vector3(_pitch, _yaw, _roll);
            var targetPitch = Mathf.Clamp(_targetRotationEuler.x + lookInput.x, _minPitch, _maxPitch);
            var targetYaw = _targetRotationEuler.y + lookInput.y;
            var targetRoll = _targetRotationEuler.z + lookInput.z;
            _targetRotationEuler = new Vector3(targetPitch, targetYaw, targetRoll);

            var smoothRotation = Vector3.SmoothDamp(currentRotation, _targetRotationEuler,
                ref _rotationVelocity, _smoothing);

            SetRotation(smoothRotation, clearSmoothing: false);
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
            SetRotation(rotationEuler);
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

            _targetRotationEuler = rotationEuler;

            _pitchPivot.transform.localRotation = Quaternion.Euler(Vector3.right * _pitch);
            _yawPivot.transform.localRotation = Quaternion.Euler(Vector3.up * _yaw);
            _rollPivot.transform.localRotation = Quaternion.Euler(Vector3.forward * _roll);
        }
    }
}
