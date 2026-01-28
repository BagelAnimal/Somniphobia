using UnityEngine;

namespace FulcrumGames.CharacterControl
{
    /// <summary>
    ///     Applies force upward when executed.
    /// </summary>
    public class Jump : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody _rigidbody;

        [SerializeField]
        private float _force = 350.0f;

        public void Execute()
        {
            if (!_rigidbody)
                return;

            var jumpDirection = transform.up;
            var jumpForce = jumpDirection * _force;
            _rigidbody.AddForce(jumpForce, ForceMode.Impulse);
        }
    }
}
