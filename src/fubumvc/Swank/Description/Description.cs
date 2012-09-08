using System;

namespace Swank.Description
{
    public class Description
    {
        public string Name { get; set; }
        public string Comments { get; set; }
        public string Namespace { get; set; }
        public Type AppliesTo { get; set; }

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
