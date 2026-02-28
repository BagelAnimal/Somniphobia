using FulcrumGames.Levels;
using FulcrumGames.Possession;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FulcrumGames.Glue
{
    /// <summary>
    ///     The intent is to provide a common access point for game state,
    ///     and to provide a top-most point for all code execution associated
    ///     with the game. In order to spin up an instance of the game,
    ///     at least one scene needs to be launched with an instance
    ///     of this component attached to its GameObject.
    /// </summary>
    public class Game : MonoBehaviour
    {
        private enum LifetimeEvent
        {
            None,
            OnAwake,
            OnStart,
            OnEnable,
            OnDisable,
            OnDestroy,
        }

        private static Game s_instance;
        public static Game Instance => s_instance;

        private Player _player;
        public Player Player => _player;

        private GameObject _playerCharacter;
        public GameObject PlayerCharacter => _playerCharacter;

        private bool _isQuitting = false;
        public static bool IsQuitting => s_instance ? s_instance._isQuitting : false;

        [SerializeField]
        private Level _levelPrefab;

        [SerializeField]
        private Player _playerPrefab;

        [SerializeField]
        private GameObject _playerCharacterPrefab;

        [SerializeField]
        private LifetimeEvent _initializeOn;

        [SerializeField]
        private LifetimeEvent _teardownOn;

        private Level _levelInstance;
        private bool _isInitialized = false;

        private void Awake()
        {
            if (s_instance)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(s_instance.gameObject);

            OnLifetimeEvent(LifetimeEvent.OnAwake);
        }

        private void Start()
        {
            OnLifetimeEvent(LifetimeEvent.OnStart);
        }

        private void OnEnable()
        {
            OnLifetimeEvent(LifetimeEvent.OnEnable);
        }


        private void OnDisable()
        {
            OnLifetimeEvent(LifetimeEvent.OnDisable);
        }

        private void OnDestroy()
        {
            OnLifetimeEvent(LifetimeEvent.OnDestroy);
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }

        private void Initialize()
        {
            try
            {
                if (_isInitialized)
                    return;

                _isInitialized = true;

                if (!_levelPrefab)
                {
                    Debug.LogError("Null level prefab in game!", this);
                    Teardown();
                    return;
                }

                if (!_playerPrefab)
                {
                    Debug.LogError("Null player soul prefab in game!", this);
                    Teardown();
                    return;
                }

                if (!_playerCharacterPrefab)
                {
                    Debug.LogError("Null player character prefab in game!", this);
                    Teardown();
                    return;
                }

                _levelInstance = Instantiate(_levelPrefab);

                _player = Instantiate(_playerPrefab);
                _player.EnableWorldControls();

                _playerCharacter = Instantiate(_playerCharacterPrefab);

                PossessionToCharacterControl.BindPlayerToCharacter(_player, _playerCharacter);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Teardown();
            }
        }

        private void Teardown()
        {
            try
            {
                if (!_isInitialized)
                    return;

                _isInitialized = false;

                if (_levelInstance)
                {
                    Destroy(_levelInstance.gameObject);
                }

                if (_player)
                {
                    Destroy(_player.gameObject);
                }

                if (_playerCharacter)
                {
                    Destroy(_playerCharacter);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void OnLifetimeEvent(LifetimeEvent executionType)
        {
            if (_initializeOn == executionType)
            {
                Initialize();
            }

            if (_teardownOn == executionType)
            {
                Teardown();
            }
        }
    }
}
