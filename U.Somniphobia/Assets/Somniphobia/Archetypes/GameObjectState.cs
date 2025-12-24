namespace FulcrumGames.Somniphobia.Archetypes
{
    [System.Serializable]
    public struct GameObjectState
    {
        public string Name;

        public string PrefabGuid;
        public string InstanceGuid;
        public string ParentGuid;

        public bool ActiveSelf;
        public int Layer;

        public float PositionX;
        public float PositionY;
        public float PositionZ;

        public float RotationX;
        public float RotationY;
        public float RotationZ;

        public float ScaleX;
        public float ScaleY;
        public float ScaleZ;

        public ISerializableState[] StatelyComponents;
    }
}
