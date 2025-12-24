using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace FulcrumGames.Somniphobia.Archetypes
{
    public class ArchetypeSerializer : MonoBehaviour
    {
        [Serializable]
        public struct Archetype
        {
            public string Name;
            public int Version;
        }

        private readonly static JsonSerializerSettings JsonSerializerSettings = new()
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            NullValueHandling = NullValueHandling.Include,
            TypeNameHandling = TypeNameHandling.All,
            Formatting = Formatting.Indented,
        };

        public static GameObject SpawnArchetype()
        {

            return null;
        }

        public void Save()
        {
            try
            {
                var directory = $"{Application.persistentDataPath}/Archetypes";
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var name = gameObject.name;
                var filePath = $"{directory}/{name}_Archetype.json";
                if (!File.Exists(filePath))
                {
                    File.CreateText(filePath).Close();
                }

                var rotationEuler = gameObject.transform.rotation.eulerAngles;
                var objectState = new GameObjectState()
                {
                    Name = name,
                    PrefabGuid = string.Empty,
                    InstanceGuid = string.Empty,
                    ParentGuid = string.Empty,
                    ActiveSelf = gameObject.activeSelf,
                    Layer = gameObject.layer,
                    PositionX = gameObject.transform.position.x,
                    PositionY = gameObject.transform.position.y,
                    PositionZ = gameObject.transform.position.z,
                    RotationX = rotationEuler.x,
                    RotationY = rotationEuler.y,
                    RotationZ = rotationEuler.z,
                    ScaleX = gameObject.transform.localScale.x,
                    ScaleY = gameObject.transform.localScale.y,
                    ScaleZ = gameObject.transform.localScale.z,
                    StatelyComponents = new ISerializableState[] { },
                };

                var filePlainText = JsonConvert.SerializeObject(objectState, JsonSerializerSettings);

                var writer = new StreamWriter(filePath, append: false);
                writer.Write(filePlainText);
                writer.Close();

                Debug.Log($"Successfully serialized archetype {name} to {filePath}.");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}