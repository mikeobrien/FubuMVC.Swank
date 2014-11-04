using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Swank.Description;
using FubuMVC.Swank.Extensions;

namespace FubuMVC.Swank.Specification
{
    public class TypeGraphFactory
    {
        private readonly Configuration _configuration;
        private readonly ITypeDescriptorCache _typeCache;
        private readonly IDescriptionConvention<PropertyInfo, MemberDescription> _memberConvention;
        private readonly OptionFactory _optionFactory;
        private readonly IDescriptionConvention<Type, TypeDescription> _typeConvention;

        public TypeGraphFactory(
            Configuration configuration, 
            ITypeDescriptorCache typeCache,
            IDescriptionConvention<Type, TypeDescription> typeConvention,
            IDescriptionConvention<PropertyInfo, MemberDescription> memberConvention,
            OptionFactory optionFactory)
        {
            _configuration = configuration;
            _typeCache = typeCache;
            _memberConvention = memberConvention;
            _optionFactory = optionFactory;
            _typeConvention = typeConvention;
        }

        public DataType BuildGraph(Type type, ActionCall action = null)
        {
            var dataType = BuildGraph(null, type, action != null, null, null, action);
            GenerateShortNamespaces(dataType);
            return dataType;
        }

        private DataType BuildGraph(
            DataType parent,
            Type type, 
            bool inputGraph,
            IEnumerable<Type> ancestors, 
            MemberDescription memberDescription = null,
            ActionCall action = null)
        {
            var description = _typeConvention.GetDescription(type);

            var dataType = new DataType
            {
                Name = !type.IsSimpleType() && memberDescription != null ? 
                    memberDescription.Name : description.Name,
                LongNamespace = parent.MapOrEmpty(x => x.LongNamespace.Concat(x.Name).ToList(), new List<string>()),
                ShortNamespace = new List<string>(),
                Comments = description.Comments
            };

            if (type.IsDictionary())
                BuildDictionary(dataType, type, description, inputGraph, ancestors, memberDescription);
            else if (type.IsArray || type.IsList())
                BuildArray(dataType, type, description, inputGraph, ancestors, memberDescription);
            else if (type.IsSimpleType()) BuildSimpleType(dataType, type);
            else BuildComplexType(dataType, type, inputGraph, ancestors, action);

            return _configuration.TypeOverrides.Apply(type, dataType);
        }

        private void BuildDictionary(
            DataType dataType,
            Type type,
            TypeDescription typeDescription,
            bool inputGraph,
            IEnumerable<Type> ancestors, 
            MemberDescription memberDescription)
        {
            var types = type.GetGenericDictionaryTypes();
            dataType.IsDictionary = true;
            dataType.Comments = memberDescription.WhenNotNull(x => x.Comments).OtherwiseDefault() ?? dataType.Comments;
            dataType.DictionaryEntry = new DictionaryEntry
            {
                KeyName = memberDescription.WhenNotNull(x => x.DictionaryEntry.KeyName).OtherwiseDefault() ??
                          typeDescription.WhenNotNull(x => x.DictionaryEntry.KeyName).OtherwiseDefault(),
                KeyComments = memberDescription.WhenNotNull(x => x.DictionaryEntry.KeyComments).OtherwiseDefault() ??
                              typeDescription.WhenNotNull(x => x.DictionaryEntry.KeyComments).OtherwiseDefault(),
                KeyType = BuildGraph(dataType, types.Key, inputGraph, ancestors),
                ValueComments = memberDescription.WhenNotNull(x => x.DictionaryEntry.ValueComments).OtherwiseDefault() ??
                                typeDescription.WhenNotNull(x => x.DictionaryEntry.ValueComments).OtherwiseDefault(),
                ValueType = BuildGraph(dataType, types.Value, inputGraph, ancestors)
            };
        }

        private void BuildArray(
            DataType dataType,
            Type type,
            TypeDescription typeDescription,
            bool inputGraph,
            IEnumerable<Type> ancestors,
            MemberDescription memberDescription)
        {
            dataType.IsArray = true;
            dataType.Comments = memberDescription.WhenNotNull(x => x.Comments).OtherwiseDefault() ?? dataType.Comments;
            var itemType = BuildGraph(dataType, type.GetListElementType(), inputGraph, ancestors);
            dataType.ArrayItem = new ArrayItem
            {
                Name = memberDescription.WhenNotNull(x => x.ArrayItem.Name).OtherwiseDefault() ??
                       typeDescription.WhenNotNull(x => x.ArrayItem.Name).OtherwiseDefault() ?? itemType.Name,
                Comments = memberDescription.WhenNotNull(x => x.ArrayItem.Comments).OtherwiseDefault() ??
                           typeDescription.ArrayItem.WhenNotNull(x => x.Comments).OtherwiseDefault(),
                Type = itemType
            };
        }

        private void BuildSimpleType(DataType dataType, Type type)
        {
            dataType.IsSimple = true;
            dataType.LongNamespace.Clear();
            dataType.ShortNamespace.Clear();
            if (type.GetNullableUnderlyingType().IsEnum) 
                dataType.Options = _optionFactory.BuildOptions(type);
        }

        private void BuildComplexType(
            DataType dataType,
            Type type,
            bool inputGraph,
            IEnumerable<Type> ancestors,
            ActionCall action)
        {
            dataType.IsComplex = true;
            dataType.Members =
                (type.IsProjection() ?
                   type.GetProjectionProperties() :
                   _typeCache.GetPropertiesFor(type).Select(x => x.Value))
                .Where(x =>
                    (!_configuration.ExcludeAutoBoundProperties || !x.IsAutoBound()) &&
                    !x.IsQuerystring(action) &&
                    !x.IsUrlParameter(action))
                .Select(x => new
                {
                    Property = x,
                    Ancestors = ancestors.Concat(type),
                    Type = x.PropertyType,
                    UnwrappedType = x.PropertyType.UnwrapType(),
                    Description = _memberConvention.GetDescription(x)
                })
                .Where(x => x.Ancestors.All(y => y != x.UnwrappedType) &&
                            !x.Description.Hidden)
                .Select(x => _configuration.MemberOverrides.Apply(x.Property, new Member
                {
                    Name = x.Description.WhenNotNull(y => y.Name).OtherwiseDefault(),
                    Comments = x.Description.WhenNotNull(y => y.Comments).OtherwiseDefault(),
                    DefaultValue = inputGraph ? x.Description.WhenNotNull(y => y.DefaultValue)
                        .WhenNotNull(z => z.ToSampleValueString(_configuration)).OtherwiseDefault() : null,
                    SampleValue =  x.Description.WhenNotNull(y => y.SampleValue)
                        .WhenNotNull(z => z.ToSampleValueString(_configuration)).OtherwiseDefault(),
                    Required = inputGraph && !x.Type.IsNullable() && x.Description.WhenNotNull(y => !y.Optional).OtherwiseDefault(),
                    Optional = inputGraph && (x.Type.IsNullable() || x.Description.WhenNotNull(y => y.Optional).OtherwiseDefault()),
                    Deprecated = x.Description.Deprecated,
                    DeprecationMessage = x.Description.DeprecationMessage,
                    Type = BuildGraph(dataType, x.Type, inputGraph, x.Ancestors, x.Description)
                })).ToList();
        }

        private static void GenerateShortNamespaces(DataType type)
        {
            type.TraverseMany(GetTypeChildTypes)
                .GroupBy(x => x.Name)
                .Where(x => x.Count() > 1)
                .ForEach(x => x.ShrinkMultipartKeyRight(y => y.LongNamespace, (t, k) => t.ShortNamespace = k));
        }

        private static IEnumerable<DataType> GetTypeChildTypes(DataType type)
        {
            if (type.Members != null)
                foreach (var childType in type.Members.Select(y => y.Type)) yield return childType;
            if (type.ArrayItem != null) yield return type.ArrayItem.Type;
            if (type.DictionaryEntry != null)
            {
                yield return type.DictionaryEntry.KeyType;
                yield return type.DictionaryEntry.ValueType;
            }
        } 
    }
}