using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Specification
{
    public class DataDescriptionFactory
    {
        public const string Whitespace = "    ";

        public List<DataDescription> Create(DataType type)
        {
            var data = new List<DataDescription>();
            WalkGraph(data, type, 1);
            return data;
        }

        public void WalkGraph(List<DataDescription> data, DataType type, int level)
        {
            if (type.IsSimple) AddSimpleType(data, type, level);
            else if (type.IsArray) WalkArray(data, type, level);
            else if (type.IsDictionary) WalkDictionary(data, type, level);
            else if (type.IsComplex) WalkComplexType(data, type, level);
        }

        public void AddSimpleType(List<DataDescription> data, DataType type, int level)
        {
        }

        public void WalkArray(List<DataDescription> data, DataType type, int level)
        {
        }

        public void WalkDictionary(List<DataDescription> data, DataType type, int level)
        {
        }

        public void WalkComplexType(List<DataDescription> data, DataType type, int level)
        {
            data.Add(new DataDescription
            {
                Name = type.Name,
                Comments = type.Comments,
                Whitespace = Whitespace.Repeat(level),
                IsOpening = true,
                IsComplexType = true
            });

            foreach (var member in type.Members)
            {
                data.Add(new DataDescription
                {
                    Name = member.Name,
                    Comments = member.Comments,
                    TypeName = member.Type.Name,
                    DefaultValue = member.DefaultValue,
                    Required = member.Required,
                    Optional = member.Optional,
                    Whitespace = Whitespace.Repeat(level + 1),
                    IsDeprecated = member.Deprecated,
                    DeprecationMessage = member.DeprecationMessage,
                    IsMember = true,
                    IsLastMember = member,

                    IsOpening = member,
                    IsClosing = member,

                });
            }

            data.Add(new DataDescription
            {
                Name = type.Name,
                Whitespace = Whitespace.Repeat(level),
                IsClosing = true,
                IsComplexType = true
            });
        }
    }
}