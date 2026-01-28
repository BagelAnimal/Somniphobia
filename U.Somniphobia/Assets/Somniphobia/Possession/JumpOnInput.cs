using UnityEngine;

namespace FulcrumGames.Possession
{
    /// <summary>
    ///     Basic jump on input to test event delegation from possession system.
    /// </summary>
    public class JumpOnInput : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private Possessable _possessable;

        [SerializeField]
        private float _force = 100.0f;

        public void Awake()
        {
            _possessable.Jump += Jump;
        }

        public void Jump()
        {
            if (!_rigidbody)
                return;

            var jumpDirection = transform.up;
            var jumpForce = jumpDirection * _force;
            _rigidbody.AddForce(jumpForce, ForceMode.Impulse);
        }
    }
}
