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

        private void Awake()
        {
            if (s_instance)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(s_instance.gameObject);
        }
    }
}
