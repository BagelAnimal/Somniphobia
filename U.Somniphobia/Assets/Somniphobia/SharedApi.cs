using UnityEngine;

namespace FulcrumGames.Somniphobia
{
    /// <summary>
    ///     A series of common game operations that might be shared across multiple
    ///     components or systems.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        ///     Teleports a <see cref="GameObject"/> to a provided <see cref="Transform"/>.
        ///     Optionally sets the object's rotation to the transform's and clears its
        ///     momentum provided that the teleporting object has a <see cref="Rigidbody"/>.
        /// </summary>
        public static void TeleportTo(GameObject teleportingObject, Transform teleportTo,
            bool setRotation = true, bool preserveMomentum = true)
        {
            if (!teleportingObject)
                return;

            if (!teleportTo)
                return;

            if (teleportingObject.TryGetComponent<Rigidbody>(out var rigidbody))
            {
                rigidbody.MovePosition(teleportTo.position);

                if (setRotation)
                {
                    rigidbody.MoveRotation(teleportTo.rotation);
                }

                if (!preserveMomentum)
                {
                    rigidbody.linearVelocity = Vector3.zero;
                    rigidbody.angularVelocity = Vector3.zero;
                }

                return;
            }

            teleportingObject.transform.position = teleportTo.position;

            if (setRotation)
            {
                teleportingObject.transform.rotation = teleportTo.rotation;
            }
        }
    }
}
