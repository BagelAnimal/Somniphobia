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

        public void Save()
        {
            var directory = $"{Application.persistentDataPath}/Archetypes";
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var name = gameObject.name;
            var filePath = $"{directory}/{name}.json";
            if (!File.Exists(filePath))
            {
                File.CreateText(filePath).Close();
            }

            var archetype = new Archetype()
            {
                Name = name,
                Version = 1
            };

            var filePlainText = JsonConvert.SerializeObject(archetype, JsonSerializerSettings);

            var writer = new StreamWriter(filePath, append: false);
            writer.Write(filePlainText);
            writer.Close();
        }
    }
}