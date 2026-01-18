using UnityEngine;
using FulcrumGames.Somniphobia.Levels;

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

        private Level _currentLevel;

        [SerializeField]
        private bool _initializeOnAwake = false;

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
                Debug.LogWarning("Attempted to initialize the game a second time! Request cancelled.");
                return;
            }

            if (!_levelPrefab)
            {
                Debug.LogWarning("Null level prefab encountered when initializing! No level will be loaded.", this);
                return;
            }

            _currentLevel = Instantiate(_levelPrefab);
            _isInitialized = true;
        }

        private void Teardown()
        {
            if (!_isInitialized)
            {
                Debug.LogWarning("Attempted to tear down the game when it was not initialized! Request cancelled.");
                return;
            }

            Destroy(_currentLevel.gameObject);
            _currentLevel = null;
            _isInitialized = false;
        }
    }
}
