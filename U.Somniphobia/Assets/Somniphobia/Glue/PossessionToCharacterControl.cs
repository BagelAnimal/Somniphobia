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
        [SerializeField]
        [Tooltip("The query distance for potential possessables in meters.")]
        private float _possessionDistance = 5.0f;

        [SerializeField]
        [Tooltip("Layers which will be checked for potential targets for possession.")]
        private LayerMask _possessableLayers = -1;

        private Look _look;
        private Walk _walk;
        private Fly _fly;
        private Jump _jump;
        private Crouch _crouch;

        private GameObject _playerCharacter;
        private Player _player;

        public bool IsControllingCharacter => _playerCharacter != null;

        private void Update()
        {
            if (!_player)
                return;

            if (_look)
            {
                _look.SetInput(_player.GetLookInput());
            }

            if (_walk)
            {
                _walk.SetInput(_player.GetMoveInput());
            }

            if (_fly)
            {
                _fly.SetInput(_player.GetMoveInput());
            }
        }

        public void BindToPlayer(Player player)
        {
            // Clear any lingering state.
            _playerCharacter = null;
            player.transform.parent = null;

            // Query relevant components from player object.
            _walk = player.GetComponent<Walk>();
            _fly = player.GetComponent<Fly>();
            _look = player.GetComponent<Look>();
            _jump = player.GetComponent<Jump>();
            _crouch = player.GetComponent<Crouch>();

            var rigidbody = player.GetComponent<Rigidbody>();
            if (rigidbody)
            {
                rigidbody.isKinematic = false;
                //rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            }

            var collider = player.GetComponent<Collider>();
            if (collider)
            {
                collider.enabled = true;
            }
        }

        public static void PossessCharacter(Player player, GameObject character)
        {
            if (!player.TryGetComponent<PossessionToCharacterControl>(out var instance))
            {
                instance = player.gameObject.AddComponent<PossessionToCharacterControl>();
                instance._player = player;
                player.InputProvided += instance.OnInput;
            }


            var anchor = character.GetComponentInChildren<PossessorAnchor>();
            var parent = anchor ? anchor.transform : character.transform;
            player.transform.parent = parent;

            // If the player's soul object has some forward stored in a look component,
            // clear state to avoid weird rotation offsets.
            if (instance._look)
            {
                instance._look.SetForward(Vector3.forward);
                instance._look.SetInput(Vector3.zero);
            }

            instance._playerCharacter = character;
            instance._walk = character.GetComponent<Walk>();
            instance._fly = character.GetComponent<Fly>();
            instance._look = character.GetComponent<Look>();
            instance._jump = character.GetComponent<Jump>();
            instance._crouch = character.GetComponent<Crouch>();

            player.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

            var rigidbody = player.GetComponent<Rigidbody>();
            if (rigidbody)
            {
                rigidbody.isKinematic = true;
                // We remove interpolation, because interpolated rigidbodies
                // do not function as expected when they are the children of other objects,
                // even when kinematic.
                rigidbody.interpolation = RigidbodyInterpolation.None;
            }

            var collider = player.GetComponent<Collider>();
            if (collider)
            {
                collider.enabled = false;
            }
        }

        public static void UnpossessCharacter(Player player, GameObject character)
        {
            if (!player.TryGetComponent<PossessionToCharacterControl>(out var instance))
                return;

            var prevLook = instance._look;
            instance.BindToPlayer(player);
            if (prevLook && instance._look)
            {
                instance._look.CopyRotationFrom(prevLook);
            }
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
                    UnpossessCharacter(_player, _playerCharacter);
                }
                else
                {
                    var forward = _look ? _look.Forward : _player.transform.forward;

                    var hits = Physics.RaycastAll(_player.transform.position, forward, _possessionDistance, _possessableLayers);
                    if (hits.Length <= 0)
                        return;

                    for (int i = 0; i < hits.Length; i++)
                    {
                        var hitObject = hits[i].collider.gameObject;
                        if (hitObject == gameObject)
                            continue;

                        if (!hitObject.TryGetComponent<Possessable>(out _))
                            continue;

                        PossessCharacter(_player, hitObject);
                        break;
                    }
                }
            }
        }
    }
}
