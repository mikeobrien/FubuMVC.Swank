using System;

namespace FubuMVC.Swank.Description
{
    public class DictionaryDescriptionAttribute : DescriptionAttribute
    {
        public DictionaryDescriptionAttribute(
            string name = null,
            string comments = null,
            string keyName = null,
            string keyComments = null,
            string valueComments = null) : base(name, comments)
        {
            KeyName = keyName;
            KeyComments = keyComments;
            ValueComments = valueComments;
        }

        public string KeyName { get; private set; }
        public string KeyComments { get; private set; }
        public string ValueComments { get; private set; }
    }
}