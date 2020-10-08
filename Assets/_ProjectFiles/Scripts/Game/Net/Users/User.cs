namespace Game.Net
{
    public sealed class User
    {
        public User(string name)
        {
            Name = name;
        }
        
        public string Name { get; private set; }
    }
}