using FulcrumGames.CharacterControl;
using FulcrumGames.Possession;
using UnityEngine;

namespace FulcrumGames.Glue
{
    /// <summary>
    ///     Binds input provided by input providers to behaviors like walk and jump
    ///     to reduce interdependency in the project.
    ///     
    ///     Very bespoke, not intended to be reused in other projects.
    /// </summary>
    public class PossessionToCharacterControl : MonoBehaviour
    {
        private Look _look;
        private Walk _walk;
        private Jump _jump;
        private Crouch _crouch;

        private GameObject _playerCharacter;
        private Player _player;

        public bool IsControllingCharacter => _playerCharacter != null;

        private void Update()
        {
            if (!_player)
                return;

            if (_walk)
            {
                _walk.SetInput(_player.GetMoveInput());
            }

            if (_look)
            {
                _look.SetInput(_player.GetLookInput());
            }
        }

        public void BindToPlayer(Player player)
        {
            _playerCharacter = null;
            _walk = player.GetComponent<Walk>();
            _look = player.GetComponent<Look>();
            _jump = player.GetComponent<Jump>();
            _crouch = player.GetComponent<Crouch>();

            player.transform.parent = null;
            var rigidbody = player.GetComponent<Rigidbody>();
            if (rigidbody)
            {
                rigidbody.isKinematic = false;
            }

            var collider = player.GetComponent<Collider>();
            if (collider)
            {
                collider.enabled = true;
            }
        }

        public static void BindPlayerToCharacter(Player player, GameObject character)
        {
            if (!player.TryGetComponent<PossessionToCharacterControl>(out var instance))
            {
                instance = player.gameObject.AddComponent<PossessionToCharacterControl>();
                instance._player = player;
                player.InputProvided += instance.OnInput;
            }

            instance._playerCharacter = character;
            instance._walk = character.GetComponent<Walk>();
            instance._look = character.GetComponent<Look>();
            instance._jump = character.GetComponent<Jump>();
            instance._crouch = character.GetComponent<Crouch>();

            var anchor = character.GetComponentInChildren<PossessorAnchor>();
            var parent = anchor ? anchor.transform : character.transform;
            player.transform.parent = parent;

            player.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            var rigidbody = player.GetComponent<Rigidbody>();
            if (rigidbody)
            {
                rigidbody.isKinematic = true;
            }

            var collider = player.GetComponent<Collider>();
            if (collider)
            {
                collider.enabled = false;
            }
        }

        public static void UnbindPlayerFromCharacter(Player player, GameObject character)
        {
            if (!player.TryGetComponent<PossessionToCharacterControl>(out var instance))
                return;

            instance.BindToPlayer(player);
        }

        private void OnInput(InputType type, InputState state)
        {
            if (type == InputType.Jump && _jump)
            {
                if (state == InputState.Pressed)
                {
                    _jump.SetInput(true);
                }
                else if (state == InputState.Released)
                {
                    _jump.SetInput(false);
                }
            }

            if (type == InputType.Crouch && _crouch)
            {
                if (state == InputState.Pressed)
                {
                    _crouch.SetInput(true);
                }
                else if (state == InputState.Released)
                {
                    _crouch.SetInput(false);
                }
            }

            if (type == InputType.Possess)
            {
                if (state != InputState.Pressed)
                    return;

                if (IsControllingCharacter)
                {
                    UnbindPlayerFromCharacter(_player, _playerCharacter);
                }
                else
                {
                    var forward = _player.transform.forward;
                    var distance = 5.0f;
                    var layerMask = -1;

                    var hits = Physics.RaycastAll(_player.transform.position, forward, distance, layerMask);
                    if (hits.Length <= 0)
                        return;

                    for (int i = 0; i < hits.Length; i++)
                    {
                        var hitObject = hits[i].collider.gameObject;
                        if (!hitObject.TryGetComponent<Possessable>(out _))
                            continue;

                        BindPlayerToCharacter(_player, hitObject);
                        return;
                    }
                }
            }
        }
    }
}
