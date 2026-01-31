using FulcrumGames.CharacterControl;
using NUnit.Framework.Interfaces;
using UnityEngine;

namespace FulcrumGames.Kinematics
{
    /// <summary>
    ///     Applies gravity to an object. May be favorable over Unity's built-in gravity
    ///     application because individuals may have their gravitational direction modified.
    /// </summary>
    [DisallowMultipleComponent]
    public class Gravity : MonoBehaviour
    {
        public const float GravityConstant = 9.81f;
        private static readonly Vector3 s_worldGravityScale = -Vector3.up;

        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private float _scalar = 1.0f;

        [SerializeField]
        private Vector3 _vector = -Vector3.up;
        public Vector3 AsVector => _vector;

        public float Magnitude => _vector.magnitude;
        public float SqrMagnitude => _vector.sqrMagnitude;
        public Vector3 Direction
        {
            get
            {
                var direction = _vector.normalized;
                direction = direction == Vector3.zero ? -transform.up : direction;
                return direction;
            }
        }

        public void FixedUpdate()
        {
            if (!_rigidbody)
            {
                Debug.LogError($"{name}'s {nameof(Gravity)}" +
                    $"is missing a {nameof(Rigidbody)}!", this);
                enabled = false;
                return;
            }

            var gravity = _scalar * GravityConstant * _vector;
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
            var currentMagnitude = _vector.magnitude;
            _vector = direction * currentMagnitude;
        }

        public void ResetDirection()
        {
            SetDirection(s_worldGravityScale);
        }

        public void SetMagnitude(float magnitude)
        {
            var direction = _vector.normalized;
            _vector = direction * magnitude;
        }

        public void ResetMagnitude()
        {
            var magnitude = s_worldGravityScale.magnitude;
            SetMagnitude(magnitude);
        }
    }
}
