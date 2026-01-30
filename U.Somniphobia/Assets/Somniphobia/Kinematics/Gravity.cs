using UnityEngine;

namespace FulcrumGames.Kinematics
{
    /// <summary>
    ///     Applies gravity to an object. May be favorable over Unity's built-in gravity
    ///     application because individuals may have their gravitational direction modified.
    /// </summary>
    public class Gravity : MonoBehaviour
    {
        public const float GravityConstant = 9.81f;
        private static readonly Vector3 s_worldGravityScale = -Vector3.up;

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private Vector3 _scale = -Vector3.up;

        private void Awake()
        {
            _rigidbody = _rigidbody ? GetComponentInChildren<Rigidbody>() : _rigidbody;
        }

        public void FixedUpdate()
        {
            var gravity = GravityConstant * _scale;
            _rigidbody.AddForce(gravity, ForceMode.Acceleration);
        }

        public void SetVector(Vector3 vector)
        {
            SetDirection(vector.normalized);
            SetMagnitude(vector.magnitude);
        }

        public void ResetVector()
        {
            ResetDirection();
            ResetMagnitude();
        }

        public void SetDirection(Vector3 vector)
        {
            var direction = vector.normalized;
            var currentMagnitude = _scale.magnitude;
            _scale = direction * currentMagnitude;
        }

        public void ResetDirection()
        {
            SetDirection(s_worldGravityScale);
        }

        public void SetMagnitude(float magnitude)
        {
            var direction = _scale.normalized;
            _scale = direction * magnitude;
        }

        public void ResetMagnitude()
        {
            var magnitude = s_worldGravityScale.magnitude;
            SetMagnitude(magnitude);
        }
    }
}
