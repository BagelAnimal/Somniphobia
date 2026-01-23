using UnityEngine;

namespace FulcrumGames.Somniphobia
{
    /// <summary>
    ///     Teleports entering objects to a serialized target position.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class TeleportTrigger : MonoBehaviour
    {
        [SerializeField]
        private Transform _teleportToAnchor;

        [SerializeField]
        private bool _preserveMomentum = true;

        [SerializeField]
        private bool _setRotation = true;

        private void OnTriggerEnter(Collider other)
        {
            var targetObject = other.attachedRigidbody
                ? other.attachedRigidbody.gameObject : other.gameObject;

            Utilities.TeleportTo(targetObject, _teleportToAnchor,
                _setRotation, _preserveMomentum);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Utilities.TeleportTo(collision.gameObject, _teleportToAnchor,
                _setRotation, _preserveMomentum);
        }
    }
}
