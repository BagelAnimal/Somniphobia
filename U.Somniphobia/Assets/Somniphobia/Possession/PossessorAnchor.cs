using UnityEngine;

namespace FulcrumGames.Possession
{
    /// <summary>
    ///     Tag component that can be used to mark a transform that possessor objects,
    ///     i.e., souls, should be a child of when a possessor is possessing this.
    ///     
    ///     Optionally tries to maintain a position for the possessor relative to the
    ///     collider's height. Useful if that collider's height is changing dynamically,
    ///     i.e., the character is crouching or changing scales.
    /// </summary>
    [DisallowMultipleComponent]
    public class PossessorAnchor : MonoBehaviour
    {
        [SerializeField]
        private BoxCollider _collider;

        [SerializeField]
        [Tooltip("Whether possessors anchored to this should update its relative position to" +
            " match collider scaling.")]
        private bool _maintainRelativeHeight = false;

        private float _distanceFromOrigin = 0.0f;

        private void Awake()
        {
            if (!_maintainRelativeHeight)
                return;

            if (!_collider)
            {
                Debug.LogError($"{name}'s {nameof(PossessorAnchor)}" +
                    $"is missing a {nameof(BoxCollider)}!", this);
                enabled = false;
                return;
            }

            var colliderHeight = _collider.size.y;
            var currentHeight = transform.position.y - _collider.transform.position.y;
            _distanceFromOrigin = currentHeight / colliderHeight;
        }

        private void LateUpdate()
        {
            if (!_maintainRelativeHeight)
                return;

            if (!_collider)
            {
                Debug.LogError($"{name}'s {nameof(PossessorAnchor)}" +
                    $"is missing a {nameof(BoxCollider)}!", this);
                enabled = false;
                return;
            }

            var colliderHeight = _collider.size.y;
            var desiredHeight = colliderHeight * _distanceFromOrigin;
            transform.position = _collider.transform.position + _collider.transform.up * desiredHeight;
        }
    }
}
