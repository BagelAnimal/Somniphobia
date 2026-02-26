using UnityEngine;

namespace FulcrumGames.Kinematics
{
    public static class GravityUtilities
    {
        public static void SetObjectGravityDirection(GameObject gameObject, Vector3 newDirection)
        {
            if (!gameObject.TryGetComponent<Gravity>(out var gravity))
            {
                gravity = gameObject.AddComponent<Gravity>();
                if (gameObject.TryGetComponent<Rigidbody>(out var rigidbody))
                {
                    rigidbody.useGravity = false;
                }
            }

            newDirection = newDirection.normalized;
            gravity.SetDirection(newDirection);
        }
    }
}
