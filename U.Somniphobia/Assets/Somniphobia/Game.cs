using FulcrumGames.Levels;
using FulcrumGames.Possession;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FulcrumGames.Somniphobia
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

        public readonly List<Player> _players = new();
        public IReadOnlyList<Player> Players => _players;
        public Player Player => _players.Count <= 0 ? null : _players[0];

        [SerializeField]
        private Level _levelPrefab;

        [SerializeField]
        private GameObject _playerCharacterPrefab;

        [SerializeField]
        private GameObject _playerSoulPrefab;

        [SerializeField]
        private LifetimeEvent _initializeOn;

        [SerializeField]
        private LifetimeEvent _teardownOn;

        private Level _levelInstance;
        private Possessor _playerSoulInstance;
        private Possessable _playerCharacterInstance;
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

        private void Initialize()
        {
            try
            {
                if (_isInitialized)
                    return;

                _isInitialized = true;

                if (!_levelPrefab)
                {
                    Debug.LogWarning("Null level prefab in game!", this);
                    return;
                }

                if (!_playerSoulPrefab)
                {
                    Debug.LogWarning("Null player soul prefab in game!", this);
                    return;
                }

                if (!_playerCharacterPrefab)
                {
                    Debug.LogWarning("Null player character prefab in game!", this);
                    return;
                }

                // Create the level.
                _levelInstance = Instantiate(_levelPrefab);

                // Create the player character within the level.
                var playerCharacterObject = Instantiate(_playerCharacterPrefab);
                if (!playerCharacterObject.TryGetComponent<Possessable>(out var possessable))
                {
                    Debug.LogWarning($"Player character instance has no possessable component!");
                    return;
                }
                _playerCharacterInstance = possessable;

                // Create the player soul, then possess the character in the level.
                var playerSoulObject = Instantiate(_playerSoulPrefab);
                if (!playerSoulObject.TryGetComponent<Possessor>(out var possessor))
                {
                    Debug.LogWarning($"Player soul refab has no possessor component!");
                    return;
                }
                _playerSoulInstance = possessor;
                _playerSoulInstance.Possess(_playerCharacterInstance);

                // Initialize player, bind to the soul that possesses character in the level.
                var hostPlayer = new Player();
                var playerName = "Host";
                hostPlayer.Initialize(name: playerName);
                _players.Add(hostPlayer);
                hostPlayer.BindToPossessor(_playerSoulInstance);
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

                foreach (var player in _players)
                {
                    player.Teardown();
                }
                _players.Clear();

                if (_levelInstance.gameObject)
                {
                    Destroy(_levelInstance.gameObject);
                }

                if (_playerSoulInstance.gameObject)
                {
                    Destroy(_playerSoulInstance.gameObject);
                }

                if (_playerCharacterInstance.gameObject)
                {
                    Destroy(_playerCharacterInstance.gameObject);
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
