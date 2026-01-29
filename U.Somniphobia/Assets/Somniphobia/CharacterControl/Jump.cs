using UnityEngine;

namespace FulcrumGames.CharacterControl
{
    /// <summary>
    ///     Applies force upward when executed.
    /// </summary>
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    public class Jump : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private float _force = 350.0f;

        private bool _input = false;

        private void Awake()
        {
            if (!_rigidbody)
            {
                _rigidbody = GetComponent<Rigidbody>();
            }
        }

        private void FixedUpdate()
        {
            if (!_input)
                return;

            if (!_rigidbody)
                return;

            var jumpDirection = transform.up;
            var jumpForce = jumpDirection * _force;
            _rigidbody.AddForce(jumpForce, ForceMode.Impulse);

            _input = false;
        }

        /// <summary>
        ///     Request a jump to later be executed by the update loop.
        /// </summary>
        public void SetInput(bool input)
        {
            _input = input;
        }
    }
}
