using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Specification
{
    public class DataDescriptionFactory
    {
        public const string Whitespace = "    ";

        private readonly Configuration _configuration;

        public DataDescriptionFactory(Configuration configuration)
        {
            _configuration = configuration;
        }

        public List<DataDescription> Create(DataType type)
        {
            var data = new List<DataDescription>();
            WalkGraph(data, type, 1);
            return data;
        }

        private void WalkGraph(List<DataDescription> data, DataType type, int level, string name = null)
        {
            if (type.IsSimple) CreateSimpleType(data, type, name, level);
            else if (type.IsArray) WalkArray(data, type, name, level);
            else if (type.IsDictionary) WalkDictionary(data, type, name, level);
            else if (type.IsComplex) WalkComplexType(data, type, name, level);
        }

        private void WalkArray(List<DataDescription> data, DataType type, string name, int level)
        {
        }

        private void WalkDictionary(List<DataDescription> data, DataType type, string name, int level)
        {
        }

        private void WalkComplexType(List<DataDescription> data, DataType type, string name, int level)
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
                var description = new DataDescription
                {
                    Name = member.Name,
                    Comments = member.Comments,
                    DefaultValue = member.DefaultValue,
                    Whitespace = Whitespace.Repeat(level + 1),
                    IsMember = true
                };

                if (member == type.Members.Last()) description.IsLastMember = true;
                if (!member.Type.IsSimple) description.IsOpening = true;
                if (member.Required) description.Required = true;
                if (member.Optional) description.Optional = true;

                if (member.Deprecated)
                {
                    description.IsDeprecated = true;
                    description.DeprecationMessage = member.DeprecationMessage;
                }

                data.Add(description);

                if (member.Type.IsSimple) DescribeSimpleType(description, member.Type, member.DefaultValue);
                if (member.Type.IsArray)
                {
                    description.IsArray = true;
                    WalkGraph(data, member.Type.ArrayItem.Type, level + 2, member.Type.ArrayItem.Name);
                }
                if (member.Type.IsDictionary)
                {
                    description.IsDictionary = true;
                    WalkGraph(data, member.Type, level + 2);
                }
                if (!member.Type.IsSimple)
                    data.Add(new DataDescription
                    {
                        Name = description.Name,
                        TypeName = description.TypeName,
                        Whitespace = description.Whitespace,
                        IsMember = true,
                        IsLastMember = description.IsLastMember,
                        IsClosing = true,
                        IsArray = description.IsArray,
                        IsDictionary = description.IsDictionary,
                        IsComplexType = description.IsComplexType
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

        private void CreateSimpleType(List<DataDescription> data, DataType type, string name, int level)
        {
            data.Add(DescribeSimpleType(new DataDescription
            {
                Name = name ?? type.Name,
                Comments = type.Comments,
                TypeName = type.Name,
                Whitespace = Whitespace.Repeat(level)
            }, type));
        }

        private DataDescription DescribeSimpleType(DataDescription data, DataType type, string defaultValue = null)
        {
            data.TypeName = type.Name;
            data.IsSimpleType = true;

            switch (type.Name)
            {
                case "unsignedLong":
                case "long":
                case "unsignedInt":
                case "int":
                case "unsignedShort":
                case "short":
                case "byte":
                case "unsignedByte":
                    data.IsNumeric = true;
                    data.DefaultValue = defaultValue ?? _configuration.SampleIntegerValue.ToDefaultValueString(_configuration);
                    break;
                case "float":
                case "double":
                case "decimal": 
                    data.IsNumeric = true;
                    data.DefaultValue = defaultValue ?? _configuration.SampleRealValue.ToDefaultValueString(_configuration);
                    break;
                case "boolean": 
                    data.IsBoolean = true;
                    data.DefaultValue = (defaultValue ?? _configuration.SampleBoolValue.ToString()).ToLower();
                    break;
                case "dateTime": 
                    data.IsDateTime = true;
                    data.DefaultValue = defaultValue ?? _configuration.SampleDateTimeValue.ToDefaultValueString(_configuration);
                   break;
                case "duration": 
                    data.IsDuration = true;
                    data.DefaultValue = defaultValue ?? _configuration.SampleTimeSpanValue.ToDefaultValueString(_configuration);
                    break;
                case "uuid": 
                    data.IsGuid = true;
                    data.DefaultValue = defaultValue ?? _configuration.SampleGuidValue.ToDefaultValueString(_configuration);
                    break;
                default: 
                    data.IsString = true;
                    data.DefaultValue = defaultValue ?? _configuration.SampleStringValue; 
                    break;
            }

            if (type.Options != null && type.Options.Any())
            {
                data.DefaultValue = type.Options.First().Value;
                data.Options = new List<Option>(type.Options.Select(x => new Option
                {
                    Name = x.Name == x.Value ? null : x.Name,
                    Value = x.Value,
                    Comments = x.Comments
                }));
            }
            return data;
        }
    }
}