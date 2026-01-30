using FulcrumGames.CharacterControl;
using FulcrumGames.Possession;
using UnityEngine;

namespace FulcrumGames.Glue
{
    /// <summary>
    ///     Binds input provided by <see cref="Possessable"/> to behaviors like <see cref="Look"/>
    ///     and <see cref="Jump"/> to reduce interdependency in the project.
    ///     
    ///     Very bespoke, not intended to be reused.
    /// </summary>
    public class PossessorToCharacterControl : MonoBehaviour
    {
        [SerializeField]
        private Possessable _possessable;

        [SerializeField]
        private Jump _jump;

        [SerializeField]
        private Walk _walk;

        [SerializeField]
        private Look _look;

        private void Awake()
        {
            _possessable = !_possessable ? GetComponentInChildren<Possessable>() : _possessable;
            _walk = !_walk ? GetComponentInChildren<Walk>() : _walk;
            _jump = !_jump ? GetComponentInChildren<Jump>() : _jump;
            _look = !_look ? GetComponentInChildren<Look>() : _look;

            _possessable.Jump += Jump;
        }

        private void OnDestroy()
        {
            if (_possessable)
            {
                _possessable.Jump -= Jump;
            }
        }

        private void Update()
        {
            if (!_possessable)
                return;

            if (_look)
            {
                var lookInput = _possessable.GetLookInput();
                _look.SetInput(lookInput);
            }

            if (_walk)
            {
                var walkInput = _possessable.GetMoveInput();
                _walk.SetInput(walkInput);
            }
        }

        private void Jump()
        {
            if (!_jump)
                return;

            _jump.SetInput(input: true);
        }
    }
}
