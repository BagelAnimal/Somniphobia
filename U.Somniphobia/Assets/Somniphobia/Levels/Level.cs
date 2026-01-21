using UnityEngine;

namespace FulcrumGames.Levels
{
    /// <summary>
    ///     A script associated to be attached to gameobjects intended
    ///     to represent levels. This is designed with a 'sceneless'
    ///     architecture in mind, where prefabs are loaded and unloaded
    ///     rather than using Unity's scenes.
    /// </summary>
    public class Level : MonoBehaviour
    {
        [SerializeField]
        private string _name = "Unnamed Level";
        public string Name => _name;

        private void Awake()
        {
            gameObject.name = _name;
        }
    }
}
