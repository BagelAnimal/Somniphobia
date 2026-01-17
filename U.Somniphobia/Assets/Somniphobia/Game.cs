using UnityEngine;

namespace FulcrumGames.Somniphobia
{
    public class Game : MonoBehaviour
    {
        private static Game s_instance;

        private void Awake()
        {
            if (s_instance)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;
        }
    }
}
