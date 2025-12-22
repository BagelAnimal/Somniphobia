using UnityEditor;
using UnityEngine;

namespace FulcrumGames.Somniphobia.Archetypes
{
    [CustomEditor(typeof(ArchetypeSerializer))]
    public class ArchetypeSerializerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Serialize"))
            {
                var archetype = (ArchetypeSerializer)target;
                archetype.Save();
            }
        }
    }
}