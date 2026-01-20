using FulcrumGames.Levels;
using FulcrumGames.Possession;
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
        private static Game s_instance;
        public static Game Instance => s_instance;

        [SerializeField]
        private Level _levelPrefab;

        [SerializeField]
        private GameObject _playerCharacterPrefab;

        [SerializeField]
        private GameObject _playerSoulPrefab;

        [SerializeField]
        private bool _initializeOnAwake = false;

        public readonly List<Player> _players = new();
        public IReadOnlyList<Player> Players => _players;
        public Player HostPlayer => _players.Count <= 0 ? null : _players[0];

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

            if (_initializeOnAwake)
            {
                Initialize();
            }
        }

        private void Initialize()
        {
            if (_isInitialized)
            {
                Debug.LogWarning("Attempted to initialize the game a second time!", this);
                return;
            }

            if (!_levelPrefab)
            {
                Debug.LogWarning("Null level prefab encountered when initializing!", this);
                return;
            }

            if (!_playerSoulPrefab)
            {
                Debug.LogWarning("Null player soul prefab encountered when initializing!", this);
                return;
            }

            if (!_playerCharacterPrefab)
            {
                Debug.LogWarning("Null player character prefab encountered when initializing!", this);
                return;
            }

            // Create the level.
            _levelInstance = Instantiate(_levelPrefab);

            // Create the player character within the level.
            var playerCharacterObject = Instantiate(_playerCharacterPrefab);
            if (!playerCharacterObject.TryGetComponent<Possessable>(out var possessable))
            {
                Debug.LogWarning($"{playerCharacterObject.name} does not have a possessable component!");
                return;
            }
            _playerCharacterInstance = possessable;

            // Create the player soul, then possess the character in the level.
            var playerSoulObject = Instantiate(_playerSoulPrefab);
            if (!playerSoulObject.TryGetComponent<Possessor>(out var possessor))
            {
                Debug.LogWarning($"{playerSoulObject.name} does not have a possessor component!");
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

            _isInitialized = true;
        }

        private void Teardown()
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("Attempted to tear down the game when it was not initialized! Request cancelled.");
                return;
            }

            Destroy(_levelInstance.gameObject);
            Destroy(_playerSoulInstance.gameObject);
            Destroy(_playerCharacterInstance.gameObject);
            foreach (var player in _players)
            {
                player.Teardown();
            }
            _players.Clear();

            _levelInstance = null;
            _isInitialized = false;
        }
    }
}
