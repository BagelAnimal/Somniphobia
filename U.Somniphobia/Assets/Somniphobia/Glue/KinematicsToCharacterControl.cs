using FulcrumGames.CharacterControl;
using FulcrumGames.Kinematics;
using UnityEngine;

namespace FulcrumGames.Somniphobia
{
    /// <summary>
    ///     Binds code related to kinematics with code related to character control.
    /// </summary>
    public class KinematicsToCharacterControl : MonoBehaviour
    {
        [SerializeField]
        private Gravity _gravity;

        [SerializeField]
        private GroundDetector _groundDetector;

        private void Awake()
        {
            _gravity = !_gravity ? GetComponentInChildren<Gravity>() : _gravity;
            _groundDetector = !_groundDetector ? GetComponentInChildren<GroundDetector>() : _groundDetector;
        }

        private void FixedUpdate()
        {
            _gravity.enabled = !_groundDetector.IsGrounded;
        }
    }
}
