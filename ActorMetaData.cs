namespace Akka.Tdd.Core
{
    public class ActorMetaData
    {
        public ActorMetaData(string name, ActorMetaData parent = null)
        {
            Name = name;
            Parent = parent;
            var parentPath = parent != null ? parent.Path : "";
            Path = Name.StartsWith("akka:") && string.IsNullOrEmpty(parentPath) ? $"{Name}" : $"{parentPath}/{Name}";
        }

        public string Name { get; }
        public ActorMetaData Parent { get; set; }
        public string Path { get; }
    }
}