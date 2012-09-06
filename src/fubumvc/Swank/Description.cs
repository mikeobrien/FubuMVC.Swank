namespace Swank
{
    public class Description
    {
        protected Description() { }

        protected Description(string name, string description = null)
        {
            Name = name;
            Comments = description;
        }

        public string Name { get; set; }
        public string Comments { get; set; }

        public override bool Equals(object obj)
        {
            return obj is Description && ((Description)obj).Name == Name;
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }
    }
}
