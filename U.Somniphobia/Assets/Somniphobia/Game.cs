using UnityEngine;

namespace FulcrumGames.Somniphobia
{
    [DisallowMultipleComponent]
    public sealed class Game : MonoBehaviour
    {
        [SerializeField]
        private GameObject _levelPrefab;

        private static Game s_instance;
        public static Game Instance => s_instance;

        private Player _player;
        public static Player Player => s_instance._player;

        private GameObject _levelInstance;

        private void Awake()
        {
            if (s_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;
        }

        private void OnEnable()
        {
            NewGame();
        }

        private void OnDisable()
        {
            Teardown();
        }

        private void NewGame()
        {
            _player = new();

            if (!_levelPrefab)
            {
                Debug.LogWarning("No level prefab has been assigned in the game!");
                return;
            }

            _levelInstance = Instantiate(_levelPrefab, Vector3.zero, Quaternion.identity);
        }

        private void Teardown()
        {
            _player = null;

            if (_levelInstance)
            {
                Destroy(_levelInstance.gameObject);
            }
        }
    }
}
