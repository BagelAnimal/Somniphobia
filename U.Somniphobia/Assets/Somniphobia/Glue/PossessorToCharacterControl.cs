using FulcrumGames.CharacterControl;
using FulcrumGames.Possession;
using UnityEngine;

namespace FulcrumGames.Glue
{
    /// <summary>
    ///     Binds input provided by <see cref="Possessable"/> to behaviors like <see cref="Look"/>
    ///     and <see cref="Jump"/> to reduce interdependency in the project.
    /// </summary>
    public class PossessorToCharacterControl : MonoBehaviour
    {
        [SerializeField]
        private Jump _jump;

        [SerializeField]
        private Look _look;

        [SerializeField]
        private Possessable _possessable;

        private void Awake()
        {
            if (!_possessable)
                return;

            _possessable.Jump += () => { if (_jump) { _jump.Execute(); } };
        }

        private void Update()
        {
            if (!_possessable)
                return;

            if (!_look)
                return;

            var lookInput = _possessable.GetLookInput();
            _look.UpdateLook(lookInput);
        }
    }
}
