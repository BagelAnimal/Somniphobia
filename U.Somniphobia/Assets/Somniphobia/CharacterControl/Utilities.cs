using UnityEngine;

namespace FulcrumGames.CharacterControl
{
    public static class Utilities
    {
        public static Vector3 XOZ(this Vector3 a) { return new Vector3(a.x, 0.0f, a.z); }
    }
}
