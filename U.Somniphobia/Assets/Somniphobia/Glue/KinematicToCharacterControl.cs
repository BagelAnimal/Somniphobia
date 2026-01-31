using FulcrumGames.CharacterControl;
using FulcrumGames.Kinematics;
using UnityEngine;

namespace FulcrumGames.Glue
{
    /// <summary>
    ///     Binds character control to kinematic behaviors like custom gravity.
    /// </summary>
    public class KinematicToCharacterControl : MonoBehaviour
    {
        [SerializeField]
        private Gravity _gravity;

        [SerializeField]
        private Jump _jump;

        [SerializeField]
        private Walk _walk;

        private void Update()
        {
            if ( _jump && _gravity)
            {
                _jump.SetGravity(_gravity.Scale);
            }

            if (_walk && _gravity)
            {
                _walk.gameObject.transform.up = -_gravity.Scale;
            }
        }

        public static void SetObjectGravityDirection(GameObject gameObject, Vector3 newDirection)
        {
            if (!gameObject.TryGetComponent<Gravity>(out var gravity))
                return;

            newDirection = newDirection.normalized;
            gravity.SetDirection(newDirection);
        }
    }
}
