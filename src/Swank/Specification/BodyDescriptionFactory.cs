using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Specification
{
    public class BodyDescriptionFactory
    {
        public const string Whitespace = "    ";

        private readonly Configuration _configuration;

        public BodyDescriptionFactory(Configuration configuration)
        {
            _configuration = configuration;
        }

        public List<BodyDescription> Create(DataType type)
        {
            var data = new List<BodyDescription>();
            WalkGraph(data, type, 0);
            return data;
        }

        private void WalkGraph(List<BodyDescription> data, DataType type, int level, 
            Action<BodyDescription> opening = null, 
            Action<BodyDescription> closing = null)
        {
            if (type.IsSimple) WalkSimpleType(data, type, level, opening);
            else if (type.IsArray) WalkArray(data, type, level, opening, closing);
            else if (type.IsDictionary) WalkDictionary(data, type, level, opening, closing);
            else if (type.IsComplex) WalkComplexType(data, type, level, opening, closing);
            if (level == 0)
            {
                data.First().IsFirst = true;
                data.Last().IsLast = true;
            }
        }

        private void WalkSimpleType(
            List<BodyDescription> description, 
            DataType type, int level,
            Action<BodyDescription> opening)
        {
            var data = new BodyDescription
            {
                Name = type.Name,
                TypeName = type.Name,
                Comments = type.Comments,
                Whitespace = Whitespace.Repeat(level),
                IsSimpleType = true
            };

            switch (type.Name)
            {
                case Xml.UnsignedLongType:
                case Xml.LongType:
                case Xml.UnsignedIntType:
                case Xml.IntType:
                case Xml.UnsignedShortType:
                case Xml.ShortType:
                case Xml.ByteType:
                case Xml.UnsignedByteType:
                    data.IsNumeric = true;
                    data.SampleValue = _configuration.SampleIntegerValue.ToDefaultValueString(_configuration);
                    break;
                case Xml.FloatType:
                case Xml.DoubleType:
                case Xml.DecimalType: 
                    data.IsNumeric = true;
                    data.SampleValue = _configuration.SampleRealValue.ToDefaultValueString(_configuration);
                    break;
                case Xml.BooleanType: 
                    data.IsBoolean = true;
                    data.SampleValue = _configuration.SampleBoolValue.ToString().ToLower();
                    break;
                case Xml.DateTimeType: 
                    data.IsDateTime = true;
                    data.SampleValue = _configuration.SampleDateTimeValue.ToDefaultValueString(_configuration);
                   break;
                case Xml.DurationType: 
                    data.IsDuration = true;
                    data.SampleValue = _configuration.SampleTimeSpanValue.ToDefaultValueString(_configuration);
                    break;
                case Xml.UuidType: 
                    data.IsGuid = true;
                    data.SampleValue = _configuration.SampleGuidValue.ToDefaultValueString(_configuration);
                    break;
                default: 
                    data.IsString = true;
                    data.SampleValue = _configuration.SampleStringValue; 
                    break;
            }

            data.Options = WalkOptions(type, x => data.SampleValue = x.First().Value);

            if (opening != null) opening(data);
            description.Add(data);
        }

        private List<Option> WalkOptions(DataType type, 
            Action<List<Option>> whenOptions = null)
        {
            List<Option> options = null;
            if (type.Options != null && type.Options.Any())
            {
                options = new List<Option>(type.Options.Select(x => new Option
                {
                    Name = x.Name == x.Value ? null : x.Name,
                    Value = x.Value,
                    Comments = x.Comments
                }));
                if (whenOptions != null) whenOptions(options);
            }
            return options;
        }

        private void WalkArray(List<BodyDescription> data, DataType type, int level,
            Action<BodyDescription> opening = null,
            Action<BodyDescription> closing = null)
        {
            var arrayOpening = new BodyDescription
            {
                Name = type.Name,
                Comments = type.Comments,
                Whitespace = Whitespace.Repeat(level),
                IsOpening = true,
                IsArray = true
            };

            if (opening != null) opening(arrayOpening);

            data.Add(arrayOpening);

            WalkGraph(data, type.ArrayItem.Type, level + 1,
                x =>
                {
                    if (type.ArrayItem != null)
                    {
                        if (type.ArrayItem.Name != null)
                            x.Name = type.ArrayItem.Name;
                        if (type.ArrayItem.Comments != null)
                            x.Comments = type.ArrayItem.Comments;
                    }
                },
                x =>
                {
                    if (type.ArrayItem != null && type.ArrayItem.Name != null)
                            x.Name = type.ArrayItem.Name;
                });

            var arrayClosing = new BodyDescription
            {
                Name = type.Name,
                Whitespace = Whitespace.Repeat(level),
                IsClosing = true,
                IsArray = true
            };

            if (closing != null) closing(arrayClosing);

            data.Add(arrayClosing);
        }

        private void WalkDictionary(List<BodyDescription> data, DataType type, int level,
            Action<BodyDescription> opening = null,
            Action<BodyDescription> closing = null)
        {
            var dictionaryOpening = new BodyDescription
            {
                Name = type.Name,
                Comments = type.Comments,
                Whitespace = Whitespace.Repeat(level),
                IsOpening = true,
                IsDictionary = true
            };

            if (opening != null) opening(dictionaryOpening);

            data.Add(dictionaryOpening);

            WalkGraph(data, type.DictionaryEntry.ValueType, level + 1,
                x =>
                {
                    x.Name = type.DictionaryEntry.KeyName ?? 
                        _configuration.DefaultDictionaryKeyName;
                    x.IsDictionaryEntry = true;
                    if (type.DictionaryEntry.ValueComments != null)
                        x.Comments = type.DictionaryEntry.ValueComments;
                    x.DictionaryKey = new Key
                    {
                        TypeName = type.DictionaryEntry.KeyType.Name,
                        Options = WalkOptions(type.DictionaryEntry.KeyType),
                        Comments = type.DictionaryEntry.KeyComments
                    };
                },
                x =>
                {
                    x.Name = _configuration.DefaultDictionaryKeyName;
                    x.IsDictionaryEntry = true;
                });

            var dictionaryClosing = new BodyDescription
            {
                Name = type.Name,
                Whitespace = Whitespace.Repeat(level),
                IsClosing = true,
                IsDictionary = true
            };

            if (closing != null) closing(dictionaryClosing);

            data.Add(dictionaryClosing);
        }

        private void WalkComplexType(List<BodyDescription> data, DataType type, int level,
            Action<BodyDescription> opening = null,
            Action<BodyDescription> closing = null)
        {
            var complexOpening = new BodyDescription
            {
                Name = type.Name,
                Comments = type.Comments,
                Whitespace = Whitespace.Repeat(level),
                IsOpening = true,
                IsComplexType = true
            };

            if (opening != null) opening(complexOpening);

            data.Add(complexOpening);

            foreach (var member in type.Members)
            {
                var lastMember = member == type.Members.Last();

                WalkGraph(data, member.Type, level + 1, 
                    x => {
                        x.Name = member.Name;
                        x.Comments = member.Comments;
                        x.DefaultValue = member.DefaultValue;
                        x.IsMember = true;
                        if (lastMember) x.IsLastMember = true;
                        if (!member.Type.IsSimple) x.IsOpening = true;
                        if (member.Required) x.Required = true;
                        if (member.Optional) x.Optional = true;

                        if (member.Deprecated)
                        {
                            x.IsDeprecated = true;
                            x.DeprecationMessage = member.DeprecationMessage;
                        }
                    }, 
                    x => {
                        x.Name = member.Name;
                        x.IsMember = true;
                        if (lastMember) x.IsLastMember = true;
                    });
            }

            var complexClosing = new BodyDescription
            {
                Name = type.Name,
                Whitespace = Whitespace.Repeat(level),
                IsClosing = true,
                IsComplexType = true
            };

            if (closing != null) closing(complexClosing);

            data.Add(complexClosing);
        }
    }
}